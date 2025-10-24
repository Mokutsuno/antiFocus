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
            phraseText.text = st == TimerState.BreakLocked ? "�[�ċz�c�i���b�N���j" : "�Ђƌċz����čĊJ���悤";
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
        remainText.text = $"�x�e {sec}s";
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        //bool locked = (timer.State == TimerState.BreakLocked);
       // resumeButton.interactable = !locked;
        extendButton.interactable = true; // �����̓��b�N���ł����i�D�݂Őؑցj
    }

    public void OnClickResume()
    {
     //   timer.ResumeFromBreak();
    }

    public void OnClickExtend()
    {
        timer.ExtendBreak(30); // +30�b
    }

    private void Show(bool on)
    {
        group.alpha = on ? 1f : 0f;
        group.blocksRaycasts = on;
        group.interactable = on;
    }
}