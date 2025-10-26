// ファイル名: DialController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AntiFocusTiemerController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] RectTransform dialRotator;     // 見た目を回すRectTransform
    [SerializeField] RectTransform angleOrigin;     // 角度計算の原点（未指定なら dialRotator）
    [SerializeField] Camera uiCamera;               // UIカメラ（スクリーン→ローカル変換用）

    [Header("Zero Offset")]
    [SerializeField, Tooltip("見た目のゼロ（真上など）をUIロジックの0°に合わせるオフセット（deg）。")]
    float zeroOffsetDeg = 90f; // アートが右=0°なら 90 で“真上=0°”

    [Header("Sweep Limits (deg)")]
    [SerializeField, Tooltip("引ける最小角（負方向、例:-300〜-360）。黒電話は時計回りに引く=負角度に進む前提")]
    float minAngle = -360f;
    [SerializeField, Tooltip("最大角（通常 0° = 原点）")]
    float maxAngle = 0f;

    [Header("Ratchet (クリック)")]
    [SerializeField, Tooltip("クリックの角度間隔。黒電話はおおよそ一定刻み")]
    float clickStepDegrees = 36f;

    [Header("Return Motion")]
    [SerializeField, Tooltip("戻り角速度（deg/sec）")]
    float returnSpeed = 540f;

    [Header("Events")]
    [Tooltip("戻り中にクリック境界を通過したら発火（カチ音などを鳴らすのに）")]
    public UnityEvent OnRatchet;
    [Tooltip("戻り終わりでクリック数→番号にマップして発火（0は10クリック扱い）")]
    public UnityEvent<int> OnDialedNumber;

    // 状態
    float currentAngle;              // 論理角（0=静止、負=引いた方向）
    float baseAngleAtStart;          // ドラッグ開始時の角度
    float lastDragLogicalAngle;      // 直前フレームのポインタ論理角
    float accumulatedDragDelta;      // ドラッグで積んだ角度差（負方向のみ採用）

    bool isReturning;
    Coroutine returnRoutine;

    // クリックカウント用
    int lastStepsDuringReturn;       // floor(-angle/step) の直前値

    void Reset()
    {
        dialRotator = GetComponent<RectTransform>();
        angleOrigin = dialRotator;
    }

    void Awake()
    {
        if (!dialRotator) dialRotator = GetComponent<RectTransform>();
        if (!angleOrigin) angleOrigin = dialRotator;
        if (!uiCamera) uiCamera = Camera.main;

        currentAngle = 0f;
        ApplyRotation(currentAngle);
    }

    // ───── Drag ───────────────────────────────────────────────────────────────

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isReturning) StopReturn();

        lastDragLogicalAngle = WorldToLogicalAngle(eventData.position);
        accumulatedDragDelta = 0f;
        baseAngleAtStart = currentAngle;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 目標（現在ポインタの論理角）
        float now = WorldToLogicalAngle(eventData.position);

        // 微小差分（-180〜+180に正規化）
        float delta = Mathf.DeltaAngle(lastDragLogicalAngle, now);
        lastDragLogicalAngle = now;

        // 黒電話：時計回り（負方向）のみ有効。正方向（戻す向き）は無視
        float negativeOnly = Mathf.Min(0f, delta);
        accumulatedDragDelta += negativeOnly;

        // ベース角＋積算を可動域にクランプ
        float target = baseAngleAtStart + accumulatedDragDelta;
        currentAngle = Mathf.Clamp(target, minAngle, maxAngle);

        ApplyRotation(currentAngle);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ほぼゼロなら終了
        if (Mathf.Abs(currentAngle) < 0.5f)
        {
            currentAngle = 0f;
            ApplyRotation(0f);
            return;
        }

        if (returnRoutine == null)
            returnRoutine = StartCoroutine(ReturnToZero());
    }

    // ───── ReturnToZero（黒電話の戻り）──────────────────────────────────────────

    IEnumerator ReturnToZero()
    {
        isReturning = true;

        // クリック段数の初期化：角度をステップで割った段数（負角なので -currentAngle）
        lastStepsDuringReturn = Mathf.FloorToInt((-currentAngle) / Mathf.Max(1e-3f, clickStepDegrees));

        while (!Mathf.Approximately(currentAngle, 0f))
        {
            float prev = currentAngle;

            // 0°に向かって一定速度で戻す（角度は増加＝0へ近づく）
            currentAngle = Mathf.MoveTowards(currentAngle, 0f, returnSpeed * Time.deltaTime);

            // クリック通過検知：floor(-angle/step) の減少を数える
            int stepsNow = Mathf.FloorToInt((-currentAngle) / Mathf.Max(1e-3f, clickStepDegrees));
            if (stepsNow < lastStepsDuringReturn)
            {
                // まとめて越える可能性に対応（非常に速い戻り・低FPSなど）
                for (int s = lastStepsDuringReturn; s > stepsNow; s--)
                    OnRatchet?.Invoke();

                lastStepsDuringReturn = stepsNow;
            }

            ApplyRotation(currentAngle);
            yield return null;
        }

        currentAngle = 0f;
        ApplyRotation(0f);

        isReturning = false;
        returnRoutine = null;

        // 総クリック数＝戻り開始時の段数（= 最初に計算しておく方が厳密だが、
        // 今回は lastStepsDuringReturn が 0 になっているので、終端で再計算）
        int totalClicks = Mathf.FloorToInt((-prevAbs) / Mathf.Max(1e-3f, clickStepDegrees));

        // ↑ prevAbs を得るため、直前ループの prev を保存しておく
        // ので、上のループ内で prevAbs を都度更新しておく
    }

    float prevAbs = 0f; // 直近フレームの |angle| を保持（総クリック数算出用）

    void LateUpdate()
    {
        if (isReturning)
        {
            // ReturnToZero 実行中は毎フレームの |角度| を保持
            prevAbs = Mathf.Abs(currentAngle);
        }
    }

    // ───── Helpers ───────────────────────────────────────────────────────────

    void StopReturn()
    {
        if (returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }
        isReturning = false;
    }

    float WorldToLogicalAngle(Vector2 screenPos)
    {
        RectTransform rect = angleOrigin ? angleOrigin : dialRotator;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect, screenPos, uiCamera, out var local);

        // Atan2(y, x) = +X軸基準。zeroOffsetDegで“見た目の真上=0°”などに合わせ、
        // DeltaAngleで [-180,180] に正規化
        float deg = Mathf.Atan2(local.y, local.x) * Mathf.Rad2Deg;
        float logical = Mathf.DeltaAngle(zeroOffsetDeg, deg);
        return logical;
    }

    void ApplyRotation(float logicalAngle)
    {
        // Z回転は画的な都合でオフセットを足す
        float visual = zeroOffsetDeg + logicalAngle;
        if (dialRotator)
            dialRotator.localRotation = Quaternion.Euler(0f, 0f, visual);
    }

    int MapClicksToDigit(int clicks)
    {
        // 黒電話は 10クリック=「0」扱い
        if (clicks <= 0) return 0;
        if (clicks == 10) return 0;
        return Mathf.Clamp(clicks, 1, 9);
    }
}
