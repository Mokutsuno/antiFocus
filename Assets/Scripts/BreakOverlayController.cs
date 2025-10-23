using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreakOverlayController : MonoBehaviour
{
    public FocusTimerController timer;
    public CanvasGroup group;           // alpha, interactable, blocksRaycasts
    public TextMeshProUGUI remainText;
    public TextMeshProUGUI phraseText;
    public Button resumeButton;
    public Button extendButton;

    private void Awake()
    {
        Show(false);
    }

    private void OnEnable()
    {
        timer.OnStateChanged += HandleState;
        timer.OnBreakRemain += HandleRemain;
    }

    private void OnDisable()
    {
        timer.OnStateChanged -= HandleState;
        timer.OnBreakRemain -= HandleRemain;
    }

    private void HandleState(TimerState st)
    {
        /**
        if (st == TimerState.BreakLocked || st == TimerState.Break)
        {
            Show(true);
            phraseText.text = st == TimerState.BreakLocked ? "深呼吸…（ロック中）" : "ひと呼吸いれて再開しよう";
            UpdateButtons();
        }
        else
        {
            Show(false);
        }
        **/
    }

    private void HandleRemain(int sec)
    {
        remainText.text = $"休憩 {sec}s";
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        //bool locked = (timer.State == TimerState.BreakLocked);
       // resumeButton.interactable = !locked;
        extendButton.interactable = true; // 延長はロック中でも許可可（好みで切替）
    }

    public void OnClickResume()
    {
     //   timer.ResumeFromBreak();
    }

    public void OnClickExtend()
    {
        timer.ExtendBreak(30); // +30秒
    }

    private void Show(bool on)
    {
        group.alpha = on ? 1f : 0f;
        group.blocksRaycasts = on;
        group.interactable = on;
    }
}