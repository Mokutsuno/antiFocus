// �t�@�C�����FDialDragHandler.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class DialDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [Header("Refs")]
    [SerializeField] RectTransform dialRotator;     // �񂷑Ώہi�ʏ�͎����j
    [SerializeField] RectTransform angleOrigin;     // �p�x�̌��_�inull�Ȃ� dialRotator�j
    [SerializeField] Camera uiCamera;               // Canvas �� Overlay�Ȃ� null �̂܂܂�OK

    [Header("Angle Limits (deg)")]
    [SerializeField] float minAngle = -120f;        // �����鉺���i���j
    [SerializeField] float maxAngle = 0f;           // ����i�ʏ�0�j

    // ����
    float startDragWorldAngle;      // ��ʁ����[�J���ϊ������J�n���̊p�x
    float baseAngleAtStart;         // �h���b�O�J�n���_�̌��݊p
    float currentAngle;             // ���p�iZ�j
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
        // �i���o�C���Ńh���b�O����ɓ���O�̈ꉟ���Ńt�H�[�J�X��͂ޖړI�B�ȗ��j
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        startDragWorldAngle = WorldToLocalAngle(eventData.position);
        baseAngleAtStart = currentAngle; // �ʏ��0
    }

    public void OnDrag(PointerEventData eventData)
    {
        float now = WorldToLocalAngle(eventData.position);
        // ������ �}180 ���ׂ��ł����肷��悤 DeltaAngle ���g�p
        float delta = Mathf.DeltaAngle(startDragWorldAngle, now);
        float target = baseAngleAtStart + delta;

        currentAngle = Mathf.Clamp(target, minAngle, maxAngle);
        ApplyRotation(currentAngle);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;

        // �����Łu��𗣂����玩���Ŗ߂��v�������ꂽ���ꍇ��
        // ������ DialController (�߂�R���[�`��) ���Ăяo�� or �����Ɏ�������B
        // ��F
        // GetComponent<DialReturner>()?.StartReturnFrom(currentAngle);
    }

    float WorldToLocalAngle(Vector2 screenPos)
    {
        RectTransform rect = angleOrigin ? angleOrigin : dialRotator;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect, screenPos, uiCamera, out var local);
        // X����0���Ƃ��Ĕ����v���BUI��Z��]�i���v��萳/���j�͌����ڂƍ��킹�₷���悤�����͌�i�Œ���
        return Mathf.Atan2(local.y, local.x) * Mathf.Rad2Deg;
    }

    void ApplyRotation(float zDeg)
    {
        dialRotator.localRotation = Quaternion.Euler(0, 0, zDeg);
    }

    float GetZ(Vector3 euler)
    {
        // 0..360 �� -180..180 �ɐ��K��
        float z = euler.z;
        if (z > 180f) z -= 360f;
        return z;
    }
}
