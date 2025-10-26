// �t�@�C��: KitchenTimer.cs
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
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
    public UnityEvent onElapsed;
    public UnityEvent<int> onTickIntSeconds; // ���b�̐����J�E���g�p�i�C�Ӂj


    public float RemainingSeconds => remainingSeconds;
    public float Normalized => maxSeconds <= 0f ? 0f : Mathf.Clamp01(remainingSeconds / maxSeconds);
    public bool IsRunning => running;

    public void SetTime(float seconds)
    {
        if (!running || allowWindWhileRunning)
        {
            remainingSeconds = Mathf.Clamp(remainingSeconds + Mathf.Max(0f, seconds), 0f, maxSeconds);
            // �����Ă��Ȃ���΁A�������玩���ő��点��i�D�݂Łj
            if (!running && remainingSeconds > 0f) StartTimer();
        }
    }

    public void StartTimer()
    {
        if (remainingSeconds > 0f) running = true;
    }

    public void StopTimer() => running = false;

    public void ResetTimer()
    {
        running = false;
        remainingSeconds = 0f;
        lastEmittedWhole = Mathf.CeilToInt(remainingSeconds);
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
                onElapsed?.Invoke();
            }
            // ���b�C�x���g�i�C�Ӂj
            int nowWhole = Mathf.CeilToInt(remainingSeconds);
            if (nowWhole != lastEmittedWhole)
            {
                lastEmittedWhole = nowWhole;
                onTickIntSeconds?.Invoke(Mathf.Max(0, nowWhole));
            }
        }
    }
}
