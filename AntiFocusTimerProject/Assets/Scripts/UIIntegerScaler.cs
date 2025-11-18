// ファイル名: UIIntegerScaler.cs
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 270x480基準のUIを整数倍スケールで表示し、フルスクリーンでもボケを防ぐ
/// Canvas:
///   - RenderMode = Screen Space - Camera
///   - CanvasScaler = Constant Pixel Size
/// Camera:
///   - UI専用カメラを使用（Overlay推奨）
/// </summary>
[RequireComponent(typeof(Camera))]
public class UIIntegerScaler : MonoBehaviour
{
    [Header("論理UI解像度（基準サイズ）")]
    public int logicalWidth = 270;
    public int logicalHeight = 480;

    [Header("対象Canvas（Constant Pixel Size推奨）")]
    public Canvas targetCanvas;
    public CanvasScaler canvasScaler;

    private Camera uiCam;
    private int lastScreenW = -1;
    private int lastScreenH = -1;

    void Awake()
    {
        uiCam = GetComponent<Camera>();

        if (!targetCanvas)
            targetCanvas = FindObjectOfType<Canvas>();

        if (!canvasScaler && targetCanvas)
            canvasScaler = targetCanvas.GetComponent<CanvasScaler>();

        if (canvasScaler)
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        ApplyScaling();
    }

    void Update()
    {
        // 解像度が変化したら再計算
        if (Screen.width != lastScreenW || Screen.height != lastScreenH)
        {
            ApplyScaling();
        }
    }

    void ApplyScaling()
    {
        lastScreenW = Screen.width;
        lastScreenH = Screen.height;

        // 画面サイズに対して整数スケールを求める
        int scaleX = Mathf.FloorToInt((float)Screen.width / logicalWidth);
        int scaleY = Mathf.FloorToInt((float)Screen.height / logicalHeight);
        int k = Mathf.Max(1, Mathf.Min(scaleX, scaleY));

        // CanvasScalerのスケールを整数値に固定
        if (canvasScaler)
            canvasScaler.scaleFactor = k;

        // UIカメラのViewportを中央寄せ
        float targetW = logicalWidth * k;
        float targetH = logicalHeight * k;
        float rectW = targetW / Screen.width;
        float rectH = targetH / Screen.height;

        Rect viewport = new Rect
        {
            width = rectW,
            height = rectH,
            x = (1f - rectW) / 2f,
            y = (1f - rectH) / 2f
        };

        uiCam.rect = viewport;

        Debug.Log($"UI整数スケール適用: {k}倍, Viewport={viewport}");
    }
}
