using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


/// <summary>
/// 円形ダイヤルをドラッグして角度→時間に変換するコンポーネント。
/// ・12時方向を0°、時計回りが正（増加）
/// ・角度のラップアラウンド（359→0や0→359）を連続量として正しく積算
/// ・1周以内で止める/複数周OK のどちらにも対応
/// ・任意分刻みでスナップ可能
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DialDragToTime : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Mapping")]
    [Tooltip("フルスケールの分数（例：60なら 1周＝60分、120なら 1周＝120分）")]
    public float fullScaleMinutes = 60f;

    [Tooltip("複数周の積算を許可（true で何周でも回せる / false で 0?fullScaleMinutes にクランプ）")]
    public bool allowMultipleTurns = false;

    [Header("Snapping")]
    [Tooltip("分単位でスナップ（0でスナップなし）")]
    [Min(0f)]
    public float snapMinutes = 0f;

    [Header("Output")]
    [Tooltip("現在値（分）。allowMultipleTurns=true の場合は 0以上の連続値、false の場合は 0?fullScaleMinutes")]
    public float currentMinutes;

    [Serializable] public class FloatEvent : UnityEvent<float> { }
    public FloatEvent OnMinutesChanged;   // UI連携用（インスペクタでハンドラを割当）

    private RectTransform rectTransform;
    private Vector2 centerScreenPos;
    private bool dragging;

    // 角度処理
    private float prevAngleDeg;   // 直前フレームの「12時基準・時計回り正」角度（0?360）
    private float totalAngleDeg;  // 積算角度（複数周分を含む連続値）

    // --- 公開API ---

    /// <summary>分をセット（外部から値を与えて針を動かしたい時など）</summary>
    public void SetMinutes(float minutes, bool invokeEvent = true)
    {
        if (!allowMultipleTurns)
        {
            minutes = Mathf.Repeat(minutes, fullScaleMinutes);
            minutes = Mathf.Clamp(minutes, 0f, fullScaleMinutes);
        }
        currentMinutes = minutes;

        // 分→角度（12時=0°, 時計回り正）
        totalAngleDeg = (currentMinutes / fullScaleMinutes) * 360f;

        if (invokeEvent) OnMinutesChanged?.Invoke(currentMinutes);
        // ここで見た目の針を回す処理があるなら呼ぶ（例：UpdateNeedleVisual()）
    }

    // --- Pointer Handlers ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        // ダイヤル中心のスクリーン座標
        Camera cam = eventData.pressEventCamera; // Screen Space - Overlay なら null でOK
        centerScreenPos = RectTransformUtility.WorldToScreenPoint(cam, rectTransform.position);

        // 現在の角度（0?360）。12時=0°, 時計回りが正
        prevAngleDeg = GetAngleFromUpClockwise(eventData.position - centerScreenPos);

        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;

        Vector2 v = eventData.position - centerScreenPos;

        // 今フレームの角度（0?360）
        float currAngleDeg = GetAngleFromUpClockwise(v);

        // 前フレームとの差を -180?+180 に正規化して、最小回転量だけを積算
        float rawDelta = currAngleDeg - prevAngleDeg;
        float deltaWrapped = Mathf.Repeat(rawDelta + 180f, 360f) - 180f;

        totalAngleDeg += deltaWrapped;
        prevAngleDeg = currAngleDeg;

        // 角度→分
        float minutes = (totalAngleDeg / 360f) * fullScaleMinutes;

        if (!allowMultipleTurns)
        {
            // 0?fullScaleMinutes に制限（負方向の回し戻しも0下回らないように）
            minutes = Mathf.Clamp(minutes, 0f, fullScaleMinutes);
            // クランプ後、totalAngleDeg も整合性を維持
            totalAngleDeg = (minutes / fullScaleMinutes) * 360f;
        }
        else
        {
            // 複数周許可時は負数も一応許容したい場合がある。負数を禁止するなら次を有効化：
            // minutes = Mathf.Max(0f, minutes);
            // totalAngleDeg = (minutes / fullScaleMinutes) * 360f;
        }

        // スナップ（分単位）
        if (snapMinutes > 0f)
        {
            minutes = Mathf.Round(minutes / snapMinutes) * snapMinutes;
            // スナップに合わせて角度側も寄せる
            totalAngleDeg = (minutes / fullScaleMinutes) * 360f;
        }

        // 反映
        currentMinutes = minutes;
        OnMinutesChanged?.Invoke(currentMinutes);

        // 必要ならここで針の見た目も更新
        // UpdateNeedleVisual(totalAngleDeg);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
    }

    // --- Utility ---

    /// <summary>
    /// 12時方向を0°、時計回りを正として 0?360° を返す。
    /// v は「中心→ポインタ」のスクリーン空間ベクトル。
    /// </summary>
    private static float GetAngleFromUpClockwise(Vector2 v)
    {
        // 通常の Atan2 は x:sin, y:cos の順に与えると 12時基準の角度が取れる
        // angleFromUpDeg: 12時=0, 右=90, 下=180, 左=270（時計回りで増加）
        float angleFromUpDeg = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;

        // 0?360 に正規化
        if (angleFromUpDeg < 0f) angleFromUpDeg += 360f;
        return angleFromUpDeg;
    }
}



//今までTimer.csとDialController.csを使用してきましたが、これらとDialDragToTime.csの関係を教えてください