// Scripts/UIBindings.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBindings : MonoBehaviour
{
    public FocusTimerController timer;

    [Header("Buttons")]
    public Button btnStart;
    public Button btnPause;
    public Button btnReset;

    [Header("Labels")]
    public TextMeshProUGUI status;
    public TextMeshProUGUI focusRemain;

    void Start()
    {
        btnStart.onClick.AddListener(timer.StartFocus);
       // btnPause.onClick.AddListener(timer.PauseFocus);
        btnReset.onClick.AddListener(timer.ResetFocus);

        timer.OnStateChanged += st => status.text = st.ToString();
        timer.OnFocusRemain += sec => focusRemain.text = $"LEFT TIME {(int)sec}s";
        timer.OnPreNotice += () => status.text = "NOTICE";
    }
}