// ファイル名：DialDragHandler.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class DialDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [Header("Refs")]
    [SerializeField] RectTransform dialRotator;     // 回す対象（通常は自分）
    [SerializeField] RectTransform angleOrigin;     // 角度の原点（nullなら dialRotator）
    [SerializeField] Camera uiCamera;               // Canvas が Overlayなら null のままでOK

    [Header("Angle Limits (deg)")]
    [SerializeField] float minAngle = -120f;        // 引ける下限（負）
    [SerializeField] float maxAngle = 0f;           // 上限（通常0）

    // 内部
    float startDragWorldAngle;      // 画面→ローカル変換した開始時の角度
    float baseAngleAtStart;         // ドラッグ開始時点の現在角
    float currentAngle;             // 実角（Z）
    bool dragging;

    void Reset()
    {
        dialRotator = GetComponent<RectTransform>();
        angleOrigin = dialRotator;
    }

    void Awake()
    {
        if (!dialRotator) dialRotator = GetComponent<RectTransform>();
        if (!angleOrigin) angleOrigin = dialRotator;
        currentAngle = GetZ(dialRotator.localEulerAngles);
        ApplyRotation(currentAngle);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // （モバイルでドラッグ判定に入る前の一押しでフォーカスを掴む目的。省略可）
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        startDragWorldAngle = WorldToLocalAngle(eventData.position);
        baseAngleAtStart = currentAngle; // 通常は0
    }

    public void OnDrag(PointerEventData eventData)
    {
        float now = WorldToLocalAngle(eventData.position);
        // 差分は ±180 を跨いでも安定するよう DeltaAngle を使用
        float delta = Mathf.DeltaAngle(startDragWorldAngle, now);
        float target = baseAngleAtStart + delta;

        currentAngle = Mathf.Clamp(target, minAngle, maxAngle);
        ApplyRotation(currentAngle);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;

        // ここで「手を離したら自動で戻す」動作を入れたい場合は
        // 既存の DialController (戻りコルーチン) を呼び出す or ここに実装する。
        // 例：
        // GetComponent<DialReturner>()?.StartReturnFrom(currentAngle);
    }

    float WorldToLocalAngle(Vector2 screenPos)
    {
        RectTransform rect = angleOrigin ? angleOrigin : dialRotator;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect, screenPos, uiCamera, out var local);
        // X軸を0°として反時計回り。UIのZ回転（時計回り正/負）は見た目と合わせやすいよう符号は後段で調整
        return Mathf.Atan2(local.y, local.x) * Mathf.Rad2Deg;
    }

    void ApplyRotation(float zDeg)
    {
        dialRotator.localRotation = Quaternion.Euler(0, 0, zDeg);
    }

    float GetZ(Vector3 euler)
    {
        // 0..360 を -180..180 に正規化
        float z = euler.z;
        if (z > 180f) z -= 360f;
        return z;
    }
}
