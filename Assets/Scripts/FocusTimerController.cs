// Scripts/FocusTimerController.cs
using System;
using UnityEngine;

public enum TimerState { Idle, FocusRunning, Break }

[DefaultExecutionOrder(-50)]
public class FocusTimerController : MonoBehaviour
{
    [Header("Durations")]
    [Range(0, 90)] public int focusMinutes = 30; // フォーカス時間
    [Range(0, 120)] public int breakSeconds = 15; // ブレイク長
    [Tooltip("フォーカス終了の予告（分）")]
    public int preNoticeMinutes = 3;

    [Header("Audio")]
    public AudioSource sfx;
    public AudioClip click;
    public AudioClip chime;

    public TimerState State { get; private set; } = TimerState.Idle;

    public event Action<TimerState> OnStateChanged;
    public event Action OnPreNotice;
    public event Action<float> OnFocusRemain; // 残り秒
    public event Action<int> OnBreakRemain;   // 残り秒

    private double _focusRemain; // 秒
    private double _breakRemain; // 秒
    private double _preNoticeAt; // 秒
    private double _lastTime;    // 秒（realTimeSinceStartup）

    private bool _noticeFired;

    void Start()
    {
        ResetFocus();
    }

    void Update()
    {
        double now = Time.realtimeSinceStartupAsDouble;
        double dt = now - _lastTime;
        _lastTime = now;

        switch (State)
        {
            case TimerState.FocusRunning:
                _focusRemain -= dt;
                OnFocusRemain?.Invoke((float)_focusRemain);   //集中時間が残ってれば、 _focusRemainをfloatにキャストする
                /*
                 invokeを使わずに書くと...
                if (OnFocusRemain != null)
                {
                    OnFocusRemain((float)_focusRemain);
                }
                以下はキャストでは？↓
                float)_focusRemain
                */
                if (!_noticeFired && _focusRemain <= _preNoticeAt)  //
                {
                    _noticeFired = true;
                    OnPreNotice?.Invoke();
                    Play(click);
                }
                if (_focusRemain <= 0)
                {
                    // EnterBreakLocked();
                    ChangeState(TimerState.Idle);
                    Stop(click);
                    Play(chime);
                }
                break;
           // case TimerState.BreakLocked:
            case TimerState.Break:
                _breakRemain -= dt;
                OnBreakRemain?.Invoke(Mathf.Max(0, Mathf.CeilToInt((float)_breakRemain)));
                /**  if (State == TimerState.BreakLocked && _breakRemain <= (breakSeconds - 15))
                  {
                      // 15秒ロック解除（開始から15秒経過）
                      ChangeState(TimerState.Break);
                  }
                **/
                break;
            case TimerState.Idle:

                break;

        }
    }

    public void StartFocus()
    {
        if (State == TimerState.Idle)
        {
            _lastTime = Time.realtimeSinceStartupAsDouble;
            ChangeState(TimerState.FocusRunning);
         //   Stop(click);
            Play(click);
        }
    }
    /*
    public void PauseFocus()
    { 
        Stop(click);
        if (State == TimerState.FocusRunning)
        {
            ChangeState(TimerState.FocusPaused);
            Play(click);
        }
        
    }
        */
    public void ResetFocus()
    {
        _focusRemain = focusMinutes; //* 60.0;
        _preNoticeAt = Mathf.Max(0, (focusMinutes - preNoticeMinutes) * 60);
        _noticeFired = false;
        _lastTime = Time.realtimeSinceStartupAsDouble;
        ChangeState(TimerState.Idle);
        Stop(click);
        OnFocusRemain?.Invoke((float)_focusRemain);
    }

    private void EnterBreakLocked()
    {
        // ブレイク開始（ロック15秒）
        _breakRemain = breakSeconds;
        _lastTime = Time.realtimeSinceStartupAsDouble;
        //ChangeState(TimerState.BreakLocked);
        Play(chime);
        ChangeState(TimerState.Idle);

    }
    /*
    public void ResumeFromBreak()
    {
        if (State == TimerState.Break)
        {
            // セッション記録
            SessionLogger.Instance?.AddSession((int)(focusMinutes * 60 - Mathf.Max(0, (float)_focusRemain)), breakSeconds);
            ResetFocus();
            Debug.Log("RESUME FROM BREAK");
            StartFocus();
        }
    }
    */
    public void ExtendBreak(int addSeconds)
    {
        //if (State == TimerState.Break || State == TimerState.BreakLocked)
        if (State == TimerState.Break)
        {
            _breakRemain += addSeconds;
        }
    }

    private void ChangeState(TimerState next)
    {
        State = next;
        OnStateChanged?.Invoke(State);
    }

    private void Play(AudioClip clip)
    {
        if (sfx != null && clip != null)
        {
            sfx.PlayOneShot(clip);
        }
    }
    private void Stop(AudioClip clip)
    {
        if (sfx != null && clip != null)
        {
            sfx.Stop();
        }
    }

}