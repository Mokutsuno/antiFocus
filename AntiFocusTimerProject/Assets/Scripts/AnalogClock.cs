// Scripts/AnalogClock.cs
using UnityEngine;
using TMPro;

public class AnalogClock : MonoBehaviour
{
    public RectTransform hourHand;
    public RectTransform minuteHand;
    public RectTransform secondHand;
    public TextMeshProUGUI digitalNow;

    void Update()
    {
        System.DateTime now = System.DateTime.Now;
        float sec = now.Second + now.Millisecond / 1000f;
        float min = now.Minute + sec / 60f;
        float hour = (now.Hour % 12) + min / 60f;

        if (hourHand) hourHand.localRotation = Quaternion.Euler(0, 0, -hour * 30f);
        if (minuteHand) minuteHand.localRotation = Quaternion.Euler(0, 0, -min * 6f);
        if (secondHand) secondHand.localRotation = Quaternion.Euler(0, 0, -sec * 6f);
        if (digitalNow) digitalNow.text = now.ToString("HH:mm:ss");
    }
}