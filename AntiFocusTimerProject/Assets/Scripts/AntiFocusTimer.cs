// ファイル: KitchenTimer.cs
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
    [Tooltip("巻ける最大時間（秒）")]
    public float maxSeconds = 3600f; // 60分
    [Tooltip("走行中の手巻き許可（キッチンタイマー実機に寄せるなら true）")]
    public bool allowWindWhileRunning = true;

    [Header("State (ReadOnly)")]
    [SerializeField] private float remainingSeconds;
    [SerializeField] private bool running;

    [Header("Events")]
    public UnityEvent onElapsed;        //経過
    public UnityEvent<int> onTickIntSeconds; // 毎秒の整数カウント用（任意）I

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
            // 走っていなければ、巻いたら自動で走らせる（好みで）
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
                onElapsed?.Invoke();        //onElapsed = 経過した　らイベント発生させる？
            }
            // 毎秒イベント（任意）
            int nowWhole = Mathf.CeilToInt(remainingSeconds);       //整数にまとめる
            if (nowWhole != lastEmittedWhole)
            {
                lastEmittedWhole = nowWhole;
                onTickIntSeconds?.Invoke(Mathf.Max(0, nowWhole));   //設定毎秒ごとにイベント発生させる？
            }
        }
    }

}
*/