/*
// ファイル名：DialController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DialController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] RectTransform dialRotator;
    [SerializeField] RectTransform angleOrigin;
    [SerializeField] Camera uiCamera;

    [Header("Zero Offset")]
    [SerializeField, Tooltip("見た目のゼロ（真上など）をUIロジックの0°に合わせるオフセット（deg）。アートが右向き=0°なら 90 を指定すると“真上=0°”になる")]
    float zeroOffsetDeg = 90f;

    [Header("Angles (deg)")]
    [SerializeField, Tooltip("引ける最小角（負方向）。例: -120°")]
    float minAngle = -360f;
    [SerializeField, Tooltip("最大角（通常 0° = 原点）")]
    float maxAngle = 0f;

    [Header("Ratchet (クリック境界)")]
    [SerializeField, Tooltip("クリック間隔（角度）。例: 12°")]
    float clickStepDegrees = 36f;

    [Header("Return Motion")]
    [SerializeField, Tooltip("戻りの角速度（deg/sec）")]
    float returnSpeed = 360f;

    [Header("Events")]
    public UnityEvent<int> OnDialedNumber;

    float currentAngle;
    float baseAngleAtStart;
    float lastDragLogicalAngle;
    float accumulatedDragDelta;

    bool isReturning;
    Coroutine returnRoutine;

    int clicksDuringReturn;
    float nextClickBoundary;

    void Reset()
    {
        dialRotator = GetComponent<RectTransform>();
        angleOrigin = dialRotator;
    }

    void Awake()
    {
        if (!dialRotator) dialRotator = GetComponent<RectTransform>();
        if (!angleOrigin) angleOrigin = dialRotator;

        currentAngle = 0f;
        ApplyRotation(currentAngle);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isReturning) StopReturn();

        lastDragLogicalAngle = WorldToLogicalAngle(eventData.position);
        accumulatedDragDelta = 0f;
        baseAngleAtStart = currentAngle;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float now = WorldToLogicalAngle(eventData.position);           // 目標となる角度
        float delta = Mathf.DeltaAngle(lastDragLogicalAngle, now);     // 微小変化分の角度
        accumulatedDragDelta += delta;                                 //蓄積分の角度         
        //lastDragLogicalAngle = now;

        float target = baseAngleAtStart + accumulatedDragDelta;

        currentAngle = Mathf.Clamp(target, minAngle, maxAngle);
        ApplyRotation(currentAngle);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(currentAngle) < 0.5f)
        {
            currentAngle = 0f;
            ApplyRotation(0f);
            return;
        }
        if (returnRoutine == null)
            returnRoutine = StartCoroutine(ReturnToZero());
    }

    IEnumerator ReturnToZero()
    {
        isReturning = true;
        clicksDuringReturn = 0;

        nextClickBoundary = currentAngle - Mathf.Repeat(currentAngle, clickStepDegrees);

        while (!Mathf.Approximately(currentAngle, 0f))
        {
            float prev = currentAngle;
            currentAngle = Mathf.MoveTowards(currentAngle, 0f, returnSpeed * Time.deltaTime);

            while (nextClickBoundary < -0.001f && currentAngle >= nextClickBoundary + clickStepDegrees)
            {
                nextClickBoundary += clickStepDegrees;
                clicksDuringReturn++;
                OnRatchetClick();
            }

            ApplyRotation(currentAngle);
            yield return null;
        }

        currentAngle = 0f;
        ApplyRotation(0f);

        isReturning = false;
        returnRoutine = null;

        int digit = MapClicksToDigit(clicksDuringReturn);
        OnDialedNumber?.Invoke(digit);
    }

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

        float deg = Mathf.Atan2(local.y, local.x) * Mathf.Rad2Deg;
        float logical = Mathf.DeltaAngle(zeroOffsetDeg, deg);
        return logical;
    }

    void ApplyRotation(float logicalAngle)
    {
        float visual = zeroOffsetDeg + logicalAngle;
        dialRotator.localRotation = Quaternion.Euler(0f, 0f, visual);
    }

    void OnRatchetClick()
    {
    }

    int MapClicksToDigit(int clicks)
    {
        if (clicks <= 0) return 0;
        if (clicks == 10) return 0;
        return Mathf.Clamp(clicks, 1, 9);
    }
}
*/
/*
// ファイル: DialController.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class DialController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Refs")]
    /// public Timer timer;
    public Transform dialVisual; // ダイアル見た目（回転させるTransform）

    [Header("Mapping")]
    [Tooltip("フルに巻いた時の目盛り角度（12時→反時計回りに何度まで行けるか）。多くの調理タイマーは1周=最大。")]
    public float maxSweepDegrees = 360f;

    [Tooltip("角度→秒の換算。例: 360度=3600秒なら 10sec/deg。")]
    public float secondsPerDegree = 10f;

    private Vector2 centerScreenPos;
    private bool dragging;
    private float accumulatedWindSeconds; // このドラッグで巻いた総秒数（指を離した時にWind）
    private Camera uiCam; // 必要なら割当（ワールド/スクリーン次第）

    private void Awake()
    {
        if (dialVisual == null) dialVisual = transform;
        uiCam = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        accumulatedWindSeconds = 0f;
        centerScreenPos = RectTransformUtility.WorldToScreenPoint(uiCam, dialVisual.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;

        // 現在角度（中心→マウス）
        Vector2 to = eventData.position - centerScreenPos;
        float angleFromUp = Mathf.Atan2(to.x, to.y) * Mathf.Rad2Deg; // 12時基準の角度（時計回りが正）
        // PointerDelta から微小角度差を算出しても良いが、ここでは event.delta を使って概算
        // より正確にするなら、前フレームの angle を保持して差分角度を積算してください。
        float deltaDegrees = eventData.delta.magnitude * Mathf.Sign(Vector3.Cross(eventData.delta, Vector3.up).z);
        // 方向を“巻く方向”のみに制限する場合は、正方向のみ加算などの制御を入れる
        float windThisFrame = Mathf.Max(0f, deltaDegrees) * secondsPerDegree;
        accumulatedWindSeconds += windThisFrame;

        // 見た目は「現在の残量 + このドラッグで増える分」に相当する角度に（プレビュー）
        float normalizedPreview = Mathf.Clamp01((timer.RemainingSeconds + accumulatedWindSeconds) / timer.maxSeconds);
        SetDialByNormalized(normalizedPreview);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!dragging) return;
        dragging = false;

        if (accumulatedWindSeconds > 0f)
        {
            timer.SetTimer(accumulatedWindSeconds);
            accumulatedWindSeconds = 0f;
        }
    }

    private void Update()
    {
        if (dragging) return; // ドラッグ中はプレビューが優先
        // 走行/停止に関わらず、残量から角度を決める
        SetDialByNormalized(timer.Normalized);
    }

    private void SetDialByNormalized(float t)
    {
        // t=1（満タン）で最大角、t=0で12時（0度）へ
        float angle = -1 * t; // 反時計回りをマイナスにする例
        dialVisual.localEulerAngles = new Vector3(0f, 0f, angle);
    }
}
*/