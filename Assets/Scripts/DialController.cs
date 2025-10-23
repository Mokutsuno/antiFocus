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
        float now = WorldToLogicalAngle(eventData.position);
        float delta = Mathf.DeltaAngle(lastDragLogicalAngle, now);
        accumulatedDragDelta += delta;
        lastDragLogicalAngle = now;

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
