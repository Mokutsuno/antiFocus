// �t�@�C��: KitchenTimer.cs
/*
using UnityEngine;
using UnityEngine.Events;

interface IDialGripper
{
    void GripDial();
}

public class AntiFocusTimerController : MonoBehaviour,IcountdownTimer
{
    [Header("Settings")]
    [Tooltip("������ő厞�ԁi�b�j")]
    public float maxSeconds = 3600f; // 60��
    [Tooltip("���s���̎芪�����i�L�b�`���^�C�}�[���@�Ɋ񂹂�Ȃ� true�j")]
    public bool allowWindWhileRunning = true;

    [Header("State (ReadOnly)")]
    [SerializeField] private float remainingSeconds;
    [SerializeField] private bool running;

    [Header("Events")]
    public UnityEvent onElapsed;        //�o��
    public UnityEvent<int> onTickIntSeconds; // ���b�̐����J�E���g�p�i�C�ӁjI

    private int lastEmittedWhole;

    public float RemainingSeconds => remainingSeconds;
    public float Normalized => maxSeconds <= 0f ? 0f : Mathf.Clamp01(remainingSeconds / maxSeconds);
    public bool IsRunning => running;
    /*
    public void SetTimer(float seconds)
    {
        if (!running || allowWindWhileRunning)
        {
            remainingSeconds = Mathf.Clamp(remainingSeconds + Mathf.Max(0f, seconds), 0f, maxSeconds);
            // �����Ă��Ȃ���΁A�������玩���ő��点��i�D�݂Łj
            if (!running && remainingSeconds > 0f) StartTick();
        }
    }
    */
/*

    public void SetRemaingingTime(float seconds)
    {

    }

    void StartTick()
    {
        if (remainingSeconds > 0f) running = true;
    }

    void StopTick() => running = false;

    void ResetTick()
    {
        running = false;
        remainingSeconds = 0f;
        lastEmittedWhole = Mathf.CeilToInt(remainingSeconds);
    }

    void ControllDial()
    {

    }
    private void Update()
    {
        if (!running) return;
        if (remainingSeconds > 0f)
        {
            remainingSeconds -= Time.deltaTime;
            if (remainingSeconds <= 0f)
            {
                remainingSeconds = 0f;
                running = false;
                onElapsed?.Invoke();        //onElapsed = �o�߂����@��C�x���g����������H
            }
            // ���b�C�x���g�i�C�Ӂj
            int nowWhole = Mathf.CeilToInt(remainingSeconds);       //�����ɂ܂Ƃ߂�
            if (nowWhole != lastEmittedWhole)
            {
                lastEmittedWhole = nowWhole;
                onTickIntSeconds?.Invoke(Mathf.Max(0, nowWhole));   //�ݒ薈�b���ƂɃC�x���g����������H
            }
        }
    }

}
*/