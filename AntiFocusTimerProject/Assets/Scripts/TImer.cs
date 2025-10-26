// ファイル: KitchenTimer.cs
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [Header("Settings")]
  //  [Tooltip("巻ける最大時間（秒）")]
  //  public float maxSeconds = 3600f; // 60分
   // [Tooltip("角度→秒の換算。例: 360度=3600秒なら 10sec/deg。")]
   // public float secondsPerDegree = 10f;
    [Tooltip("走行中の手巻き許可（キッチンタイマー実機に寄せるなら true）")]
  //  public bool allowWindWhileRunning = true;

    [Header("State (ReadOnly)")]
    [SerializeField] private float remainingSeconds;
    [SerializeField] private bool running;


    //[SerializeField] private bool secondsPerDegree;




    //  [Header("Events")]
    //  public UnityEvent onElapsed;
    //  public UnityEvent<int> onTickIntSeconds; // 毎秒の整数カウント用（任意）

    private int lastEmittedWhole;

   // public float RemainingSeconds => remainingSeconds;
  //  public float Normalized => maxSeconds <= 0f ? 0f : Mathf.Clamp01(remainingSeconds / maxSeconds);
    public bool IsRunning => running;
    /*
    public void SetTimer(float seconds)
    {
        if (!running || allowWindWhileRunning)
        {
            remainingSeconds = Mathf.Clamp(remainingSeconds + Mathf.Max(0f, seconds), 0f, maxSeconds);
            // 走っていなければ、巻いたら自動で走らせる（好みで）
            if (!running && remainingSeconds > 0f) StartTimer();
        }
    }*/
    /*
    public void StartTimer()
    {
        if (remainingSeconds > 0f) running = true;
    }*/
    /*
    public void StopTimer() => running = false;
    */
    /*
    public void ResetTimer()
    {
        running = false;
        remainingSeconds = 0f;
        lastEmittedWhole = Mathf.CeilToInt(remainingSeconds);
    }*/
    /*
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
            // 毎秒イベント（任意）
            int nowWhole = Mathf.CeilToInt(remainingSeconds);
            if (nowWhole != lastEmittedWhole)
            {
                lastEmittedWhole = nowWhole;
                onTickIntSeconds?.Invoke(Mathf.Max(0, nowWhole));
            }
        }
    }
    */
}
