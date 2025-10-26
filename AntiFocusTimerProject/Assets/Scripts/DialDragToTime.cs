using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


/// <summary>
/// �~�`�_�C�������h���b�O���Ċp�x�����Ԃɕϊ�����R���|�[�l���g�B
/// �E12��������0���A���v��肪���i�����j
/// �E�p�x�̃��b�v�A���E���h�i359��0��0��359�j��A���ʂƂ��Đ������ώZ
/// �E1���ȓ��Ŏ~�߂�/������OK �̂ǂ���ɂ��Ή�
/// �E�C�ӕ����݂ŃX�i�b�v�\
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DialDragToTime : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Mapping")]
    [Tooltip("�t���X�P�[���̕����i��F60�Ȃ� 1����60���A120�Ȃ� 1����120���j")]
    public float fullScaleMinutes = 60f;

    [Tooltip("�������̐ώZ�����itrue �ŉ����ł��񂹂� / false �� 0?fullScaleMinutes �ɃN�����v�j")]
    public bool allowMultipleTurns = false;

    [Header("Snapping")]
    [Tooltip("���P�ʂŃX�i�b�v�i0�ŃX�i�b�v�Ȃ��j")]
    [Min(0f)]
    public float snapMinutes = 0f;

    [Header("Output")]
    [Tooltip("���ݒl�i���j�BallowMultipleTurns=true �̏ꍇ�� 0�ȏ�̘A���l�Afalse �̏ꍇ�� 0?fullScaleMinutes")]
    public float currentMinutes;

    [Serializable] public class FloatEvent : UnityEvent<float> { }
    public FloatEvent OnMinutesChanged;   // UI�A�g�p�i�C���X�y�N�^�Ńn���h���������j

    private RectTransform rectTransform;
    private Vector2 centerScreenPos;
    private bool dragging;

    // �p�x����
    private float prevAngleDeg;   // ���O�t���[���́u12����E���v��萳�v�p�x�i0?360�j
    private float totalAngleDeg;  // �ώZ�p�x�i�����������܂ޘA���l�j

    // --- ���JAPI ---

    /// <summary>�����Z�b�g�i�O������l��^���Đj�𓮂����������Ȃǁj</summary>
    public void SetMinutes(float minutes, bool invokeEvent = true)
    {
        if (!allowMultipleTurns)
        {
            minutes = Mathf.Repeat(minutes, fullScaleMinutes);
            minutes = Mathf.Clamp(minutes, 0f, fullScaleMinutes);
        }
        currentMinutes = minutes;

        // �����p�x�i12��=0��, ���v��萳�j
        totalAngleDeg = (currentMinutes / fullScaleMinutes) * 360f;

        if (invokeEvent) OnMinutesChanged?.Invoke(currentMinutes);
        // �����Ō����ڂ̐j���񂷏���������Ȃ�Ăԁi��FUpdateNeedleVisual()�j
    }

    // --- Pointer Handlers ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        // �_�C�������S�̃X�N���[�����W
        Camera cam = eventData.pressEventCamera; // Screen Space - Overlay �Ȃ� null ��OK
        centerScreenPos = RectTransformUtility.WorldToScreenPoint(cam, rectTransform.position);

        // ���݂̊p�x�i0?360�j�B12��=0��, ���v��肪��
        prevAngleDeg = GetAngleFromUpClockwise(eventData.position - centerScreenPos);

        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;

        Vector2 v = eventData.position - centerScreenPos;

        // ���t���[���̊p�x�i0?360�j
        float currAngleDeg = GetAngleFromUpClockwise(v);

        // �O�t���[���Ƃ̍��� -180?+180 �ɐ��K�����āA�ŏ���]�ʂ�����ώZ
        float rawDelta = currAngleDeg - prevAngleDeg;
        float deltaWrapped = Mathf.Repeat(rawDelta + 180f, 360f) - 180f;

        totalAngleDeg += deltaWrapped;
        prevAngleDeg = currAngleDeg;

        // �p�x����
        float minutes = (totalAngleDeg / 360f) * fullScaleMinutes;

        if (!allowMultipleTurns)
        {
            // 0?fullScaleMinutes �ɐ����i�������̉񂵖߂���0�����Ȃ��悤�Ɂj
            minutes = Mathf.Clamp(minutes, 0f, fullScaleMinutes);
            // �N�����v��AtotalAngleDeg �����������ێ�
            totalAngleDeg = (minutes / fullScaleMinutes) * 360f;
        }
        else
        {
            // �����������͕������ꉞ���e�������ꍇ������B�������֎~����Ȃ玟��L�����F
            // minutes = Mathf.Max(0f, minutes);
            // totalAngleDeg = (minutes / fullScaleMinutes) * 360f;
        }

        // �X�i�b�v�i���P�ʁj
        if (snapMinutes > 0f)
        {
            minutes = Mathf.Round(minutes / snapMinutes) * snapMinutes;
            // �X�i�b�v�ɍ��킹�Ċp�x�����񂹂�
            totalAngleDeg = (minutes / fullScaleMinutes) * 360f;
        }

        // ���f
        currentMinutes = minutes;
        OnMinutesChanged?.Invoke(currentMinutes);

        // �K�v�Ȃ炱���Őj�̌����ڂ��X�V
        // UpdateNeedleVisual(totalAngleDeg);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
    }

    // --- Utility ---

    /// <summary>
    /// 12��������0���A���v���𐳂Ƃ��� 0?360�� ��Ԃ��B
    /// v �́u���S���|�C���^�v�̃X�N���[����ԃx�N�g���B
    /// </summary>
    private static float GetAngleFromUpClockwise(Vector2 v)
    {
        // �ʏ�� Atan2 �� x:sin, y:cos �̏��ɗ^����� 12����̊p�x������
        // angleFromUpDeg: 12��=0, �E=90, ��=180, ��=270�i���v���ő����j
        float angleFromUpDeg = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;

        // 0?360 �ɐ��K��
        if (angleFromUpDeg < 0f) angleFromUpDeg += 360f;
        return angleFromUpDeg;
    }
}



//���܂�Timer.cs��DialController.cs���g�p���Ă��܂������A������DialDragToTime.cs�̊֌W�������Ă�������