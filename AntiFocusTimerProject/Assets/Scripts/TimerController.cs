using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent (typeof(AudioSource))]
public class TimerController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("巻ける最大時間（秒）")]
    float maxSeconds = 3600f;

    [Header("State (ReadOnly)")]
    [SerializeField] private float remainingSeconds; // 残り時間
    [SerializeField] private bool running;

    [Header("Events")]
    public UnityEvent onElapsed;                 // 0になった時
    public UnityEvent<int> onTickIntSeconds;     // 毎秒コールバック（任意）

    [Header("Refs")]
    public Transform secDialVisual;                 // ダイアル見た目（回転させるTransform）
    public Transform minDialVisual;                 // ダイアル見た目（回転させるTransform）
    public Transform hourDialVisual;                 // ダイアル見た目（回転させるTransform）
    public Transform keepDialVisual;                 // ダイアル見た目（回転させるTransform）
    [Tooltip("タイマーの軸の中心（UIの場合にRectTransformで指定）")]
    public RectTransform dialAxis;

    public AudioSource tickAudioSource;
    public AudioSource bellAudioSource;

    public Text text;
    // 内部
    private Camera uiCam;                        // 必要なら割当（UIカメラ/メインカメラ）
    private int lastEmittedWhole;               //キッチンタイマーのように時間がカウントダウンしていくときに、1秒ごとにイベントを発火させるために使われています。

    // ドラッグ関連
    private bool isDragging;
    private float startRemainingSeconds;         // ドラッグ開始時の残り時間
    private float accumulatedWindSeconds;        // ドラッグ中に増減した秒
    private float prevAngleCW;                   // 前フレーム角度（12時基準/時計回り正）

    // 1度あたりの秒（maxSeconds を 360度に割り当て）
    private float SecondsPerDegree => maxSeconds / 360f;

   
    private void Awake()
    {
      //  tickAudioSource = GetComponent<AudioSource>();

       // text.text = WriteTimeFormat(remainingSeconds);
      //  if (dialVisual == null) dialVisual = transform;
        uiCam = Camera.main;
        ApplyDialFromSeconds(remainingSeconds);
        lastEmittedWhole = Mathf.CeilToInt(remainingSeconds);
    }

    //floatの値をタイム表記の文字列で返す
    public static string WriteTimeFormat(float time)
    {
        string timeString = string.Format("{0:D2}:{1:D2}:{2:D2}",
            (int)time / 60,
            (int)time % 60,
            (int)(time * 100) % 60);
        return timeString;
    }
    private void Update()
    {
        text.text = WriteTimeFormat(remainingSeconds);
        if (!running) return;

        if (remainingSeconds > 0f)
        {
            remainingSeconds -= Time.deltaTime*10;
            if (remainingSeconds <= 0f)
            {
                remainingSeconds = 0f;
                running = false;
                bellAudioSource.Play();
                onElapsed?.Invoke();
            }

            // ダイアルを残り時間に合わせて戻す（＝キッチンタイマーの針が12時に戻る）
            ApplyDialFromSeconds(remainingSeconds);
            ApplyDialFromSeconds(remainingSeconds);

            // 毎秒イベント（任意）
            int nowWhole = Mathf.CeilToInt(remainingSeconds);
            if (nowWhole != lastEmittedWhole)
            {
                tickAudioSource.Play();
                lastEmittedWhole = nowWhole;
                onTickIntSeconds?.Invoke(Mathf.Max(0, nowWhole));
            }
        }
    }

    //=== Drag API（EventTriggerや独自呼び出し想定） =========================

    public void OnDragStart(PointerEventData e)
    {
        isDragging = true;
        bellAudioSource.Stop();


        // カウントダウン一時停止（キッチンタイマーを巻いている間は停止する想定）
        bool wasRunning = running;
        running = false;

        // 直前の状態を保存
        startRemainingSeconds = remainingSeconds;
        accumulatedWindSeconds = 0f;

        // 直近角度初期化
        prevAngleCW = AngleFrom12Clockwise(GetPointerFromCenter(e.position));

        // 再開予定のフラグは不要。DragEndで remaining を確定して start する。
    }

    public void OnDragUpdate(PointerEventData e)
    {
        if (!isDragging) return;

        // 現在角度（12時基準/時計回りが正）
        float angleCW = AngleFrom12Clockwise(GetPointerFromCenter(e.position));

        // 最短角度差（-180〜+180）
        float deltaDeg = Mathf.DeltaAngle(prevAngleCW, angleCW);
        prevAngleCW = angleCW;

        // 角度→秒に変換して累積（時計回りで+、反時計回りで-）
        accumulatedWindSeconds += deltaDeg * SecondsPerDegree;

        // プレビュー（0〜maxにクランプ）
        float previewSeconds = Mathf.Clamp(startRemainingSeconds + accumulatedWindSeconds, 0f, maxSeconds);

        // ダイアルに即時反映（プレビュー）
        ApplyDialFromSeconds(previewSeconds);
    }

    public void OnDragEnd(PointerEventData e)
    {
        if (!isDragging) return;
        isDragging = false;

        // ドラッグで確定した残り時間をセット
        float newSeconds = Mathf.Clamp(startRemainingSeconds + accumulatedWindSeconds, 0f, maxSeconds);
        accumulatedWindSeconds = 0f;

        remainingSeconds = newSeconds;

        // 0より大きければ、ここから自動で「12時へ戻る＝減っていく」挙動（Update内）を開始
        running = remainingSeconds > 0f;

        // 針の初期位置を合わせておく
        ApplyDialFromSeconds(remainingSeconds);

        // 毎秒イベントの起点を同期
        lastEmittedWhole = Mathf.CeilToInt(remainingSeconds);

    }

    //=== Utility ============================================================

    // 12時基準/時計回り正の角度取得
    private static float AngleFrom12Clockwise(Vector2 fromCenter)
    {
        float deg = Mathf.Atan2(fromCenter.x, fromCenter.y) * Mathf.Rad2Deg;
        return Mathf.Repeat(deg, 360f);
    }

    // 中心からポインタへのベクトル（スクリーン座標前提）
    private Vector2 GetPointerFromCenter(Vector2 pointerScreenPos)
    {
        //Vector2 center = dialAxis != null
        //   ? (Vector2)dialAxis.position
        // : (Vector2)dialVisual.position;
        Vector2 center = dialAxis.position;
        return pointerScreenPos - center;
    }

    // 残り秒からダイアルの角度に反映（t=0で12時, t=maxで-360°）
    private void ApplyDialFromSeconds(float seconds)
    {
        float t01 = (maxSeconds <= 0f) ? 0f : Mathf.Clamp01(seconds / maxSeconds*0.5f);
        if(t01 == 0)
        {
           //tickudioSource.PlayOneShot(soundBell);

        }
        float angleZ = -t01 * 360f;
        secDialVisual.localEulerAngles = new Vector3(0f, 0f, angleZ*3600*2);
        minDialVisual.localEulerAngles = new Vector3(0f, 0f, angleZ*60*2);
        hourDialVisual.localEulerAngles = new Vector3(0f, 0f, angleZ*2);
        if(isDragging&&!running) keepDialVisual.localEulerAngles = new Vector3(0f, 0f, angleZ*2);
    }
    //=== Public API（任意で使いたい場合） ==================================

    public void SetRemainingTime(float seconds, bool startImmediately = true)
    {
        remainingSeconds = Mathf.Clamp(seconds, 0f, maxSeconds);
        ApplyDialFromSeconds(remainingSeconds);
        running = startImmediately && remainingSeconds > 0f;
        lastEmittedWhole = Mathf.CeilToInt(remainingSeconds);
    }

    public void Stop()
    {
        running = false;
    }

    public float RemainingSeconds => remainingSeconds;
    public bool IsRunning => running;
}
