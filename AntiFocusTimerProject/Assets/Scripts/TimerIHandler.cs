using UnityEngine;
using UnityEngine.EventSystems;
//sing static Unity.VisualScripting.Round<TInput, TOutput>;

public class TimerHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    
    [SerializeField] private TimerController timerController;


    public void OnPointerDown(PointerEventData eventData)
    {
        timerController.OnDragStart(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        timerController.OnDragUpdate(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        timerController.OnDragEnd();
    }

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
}