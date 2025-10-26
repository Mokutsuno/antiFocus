using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TimerController : MonoBehaviour
{

    [Header("Settings")]
    [Tooltip("巻ける最大時間（秒）")]
    public float maxSeconds = 3600f; // 60分
   // [Tooltip("走行中の手巻き許可（キッチンタイマー実機に寄せるなら true）")]
  //  public bool allowWindWhileRunning = true;

    [Header("State (ReadOnly)")]
    [SerializeField] private float remainingSeconds; //ダイヤルを回して指を話したら減り始める時間のこと
    [SerializeField] private bool running;

    [Header("Events")]
    public UnityEvent onElapsed;        //経過
    public UnityEvent<int> onTickIntSeconds; // 毎秒の整数カウント用（任意）I

    [Header("Refs")]
    /// public Timer timer;
    public Transform dialVisual; // ダイアル見た目（回転させるTransform）

   // [Header("Mapping")]
   // [Tooltip("フルに巻いた時の目盛り角度（12時→反時計回りに何度まで行けるか）。多くの調理タイマーは1周=最大。")]
    //public float maxSweepDegrees = 360f;

    [Tooltip("角度→秒の換算。例: 360度=3600秒なら 10sec/deg。")]
    public float secondsPerDegree = 10f;


      private bool allowWindWhileRunning = true;
    [Tooltip("タイマーの軸の中心")]
    public RectTransform dialAxis;
    private bool dragging;
    private float accumulatedWindSeconds; // このドラッグで巻いた総秒数（指を離した時にWind）
    private Camera uiCam; // 必要なら割当（ワールド/スクリーン次第）

    private float startRemainingSeconds;      // ドラッグ開始時の残り時間

    //[SerializeField] float maxSeconds = 300f; // 例：5分＝300秒
    private float SecondsPerDegree => maxSeconds / 3600f;


    private int lastEmittedWhole;

    public float RemainingSeconds => remainingSeconds;
    //public float Normalized => maxSeconds <= 0f ? 0f : Mathf.Clamp01(remainingSeconds / maxSeconds);
    //public bool IsRunning => running;
    private float prevAngleCW;                // 前フレームの角度(12時基準/時計回り正)
    private void Awake()
    {
        if (dialVisual == null) dialVisual = transform;
        uiCam = Camera.main;
    }
    public void SetRemaingingTime(float seconds)
    {

    }

    void StopTick() => running = false;

    void ResetTick()
    {
        running = false;
        remainingSeconds = 0f;
        lastEmittedWhole = Mathf.CeilToInt(remainingSeconds);
    }

    void ControllDial()
    {

    }
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
                onElapsed?.Invoke();        //onElapsed = 経過した　らイベント発生させる？
            }
            // 毎秒イベント（任意）
            int nowWhole = Mathf.CeilToInt(remainingSeconds);       //整数にまとめる
            if (nowWhole != lastEmittedWhole)
            {
                lastEmittedWhole = nowWhole;
                onTickIntSeconds?.Invoke(Mathf.Max(0, nowWhole));   //設定毎秒ごとにイベント発生させる？
            }
        }
    }

    private float currentTime;
    private bool isDragging;

    public void OnDragStart(PointerEventData e)
    {
        Debug.Log("OnDragStart");
        if (remainingSeconds > 0f) running = true;
        isDragging = true;
    }
    private static float AngleFrom12Clockwise(Vector2 fromCenter)
    {
        // 通常のatan2はx基準(右が0°、反時計回りが正)なので
        // 12時基準/時計回り正にするため xとyを入れ替えつつ符号を反転
        float deg = -Mathf.Atan2(fromCenter.x, fromCenter.y) * Mathf.Rad2Deg;
        return Mathf.Repeat(deg, 360f);
    }
    public void OnDragUpdate(PointerEventData e)
    {
        Debug.Log("OnDragUpdate");
        if (!isDragging) return;

        Vector2 to = e.position - new Vector2(dialAxis.position.x, dialAxis.position.y);

        // 現在角度(12時基準/時計回り正)
        float angleCW = AngleFrom12Clockwise(to);

        // 前フレーム→現在の最短角度差（符号付き, -180〜+180）
        float deltaDeg = Mathf.DeltaAngle(prevAngleCW, angleCW);
        prevAngleCW = angleCW;

        // 時計回りのみを「ゼンマイを巻く」とみなすなら Max(0, deltaDeg)
        // 双方向で時間を増減させたいなら ↓の行のコメントアウトを外し、上をコメントアウトする
        //float windDeg = Mathf.Max(0f, deltaDeg);
        float windDeg = deltaDeg;

        // 角度→秒に変換して累積
        accumulatedWindSeconds += windDeg * SecondsPerDegree;
        SetTimer(accumulatedWindSeconds/360f*-1f);
        // プレビュー用の残り時間（0〜maxSecondsにクランプ）
        float previewSeconds = Mathf.Clamp(startRemainingSeconds + accumulatedWindSeconds, 0f, maxSeconds);

        // 正規化してダイヤル更新（0〜1）
        float normalized = previewSeconds / maxSeconds;
        //Debug.Log(normalized);

    }
    public void OnDragEnd()
    {
        Debug.Log("OnDragEnd");
        if (!dragging) return;
        dragging = false;

        if (accumulatedWindSeconds > 0f)
        {
            SetTimer(accumulatedWindSeconds);
            accumulatedWindSeconds = 0f;
        }

    }
    private void StartCountdown()
    {
        // 実際のタイマー開始ロジック
    }

    private void SetTimer(float t)
    {
        Debug.Log("t=" + t);
        // t=1（満タン）で最大角、t=0で12時（0度）へ
        float angle = -1 * t*360; // 反時計回りをマイナスにする例
        dialVisual.localEulerAngles = new Vector3(0f, 0f, angle);
    }
}
