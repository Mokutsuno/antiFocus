// Scripts/FocusTimerController.cs
using System;
using UnityEngine;

public enum TimerState { Idle, FocusRunning, Break }

[DefaultExecutionOrder(-50)]
public class FocusTimerController : MonoBehaviour
{
    [Header("Durations")]
    [Range(0, 90)] public int focusMinutes = 30; // �t�H�[�J�X����
    [Range(0, 120)] public int breakSeconds = 15; // �u���C�N��
    [Tooltip("�t�H�[�J�X�I���̗\���i���j")]
    public int preNoticeMinutes = 3;

    [Header("Audio")]
    public AudioSource sfx;
    public AudioClip click;
    public AudioClip chime;

    public TimerState State { get; private set; } = TimerState.Idle;

    public event Action<TimerState> OnStateChanged;
    public event Action OnPreNotice;
    public event Action<float> OnFocusRemain; // �c��b
    public event Action<int> OnBreakRemain;   // �c��b

    private double _focusRemain; // �b
    private double _breakRemain; // �b
    private double _preNoticeAt; // �b
    private double _lastTime;    // �b�irealTimeSinceStartup�j

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
                OnFocusRemain?.Invoke((float)_focusRemain);   //�W�����Ԃ��c���Ă�΁A _focusRemain��float�ɃL���X�g����
                /*
                 invoke���g�킸�ɏ�����...
                if (OnFocusRemain != null)
                {
                    OnFocusRemain((float)_focusRemain);
                }
                �ȉ��̓L���X�g�ł́H��
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
                      // 15�b���b�N�����i�J�n����15�b�o�߁j
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
        // �u���C�N�J�n�i���b�N15�b�j
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
            // �Z�b�V�����L�^
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