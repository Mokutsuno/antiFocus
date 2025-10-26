// ファイル名：DialGizmoVisualizer.cs
// 目的：DialController の clickStepDegrees / minAngle / maxAngle に基づいて
//       Scene ビュー上に「クリック境界（等角度の目盛り）」を表示する。
// 使い方：DialController と同じ GameObject に付け、必要なら半径や色を調整。
/*
using UnityEngine;

[ExecuteAlways]
public class DialGizmoVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] DialController dialController;   // 同じGO推奨
    [SerializeField] RectTransform dialOrigin;        // 角度の基準（通常 DialRotator）
                                                      // 未設定なら dialController の angleOrigin or dialRotator を自動使用

    [Header("Appearance")]
    [SerializeField, Tooltip("円周の可視化半径（UIピクセル相当）。未設定時はRectの短辺/2を使用")]
    float radiusOverride = 0f;
    [SerializeField, Tooltip("目盛り線の長さ（内向き）")]
    float tickLength = 20f;
    [SerializeField] Color rimColor = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] Color tickColor = new Color(1f, 0.65f, 0f, 0.9f);
    [SerializeField] bool showLabels = true;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (dialController == null) dialController = GetComponent<DialController>();
        if (dialController == null) return;

        // 参照解決
        RectTransform origin = dialOrigin;
        if (origin == null)
        {
            // DialController 内の angleOrigin または dialRotator を流用
            var angleOriginField = typeof(DialController).GetField("angleOrigin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dialRotatorField = typeof(DialController).GetField("dialRotator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            origin = (RectTransform)(angleOriginField?.GetValue(dialController) as RectTransform)
                     ?? (RectTransform)(dialRotatorField?.GetValue(dialController) as RectTransform);
        }
        if (origin == null) return;

        // 角度パラメータ取得
        float step = GetPrivateFloat(dialController, "clickStepDegrees", 12f);
        float minAngle = GetPrivateFloat(dialController, "minAngle", -120f);
        float maxAngle = GetPrivateFloat(dialController, "maxAngle", 0f);
        float zeroOff = GetPrivateFloat(dialController, "zeroOffsetDeg", 45f); // ★ 追加
        if (step <= 0.0001f) return;

        // 半径決定（UI想定：Rect の短辺/2）
        float radius = radiusOverride > 0f ? radiusOverride : Mathf.Min(origin.rect.width, origin.rect.height) * 0.5f;

        // 原点（ワールド）
        Vector3 center = origin.TransformPoint(Vector3.zero);

        // 円周（リム）を薄く描画
        UnityEditor.Handles.color = rimColor;
        DrawCircle(origin, center, radius);

        // 目盛り（クリック境界）を描画：maxAngle(通常0°) → minAngle(負) まで step 刻み
        UnityEditor.Handles.color = tickColor;

        int tickIndex = 0;
        for (float a = maxAngle; a >= minAngle - 0.001f; a -= step)
        {
            // ★ “真上＝0°”のロジック角 a に zeroOffset を足して見た目角へ
            Vector3 localDir = Quaternion.Euler(0f, 0f, zeroOff + a) * Vector3.right;

            Vector3 worldOuter = origin.TransformPoint(localDir * radius);
            Vector3 worldInner = origin.TransformPoint(localDir * (radius - tickLength));
            UnityEditor.Handles.DrawAAPolyLine(2f, new Vector3[] { worldOuter, worldInner });

            if (showLabels)
            {
                // ★ クリック数→電話の数字（10クリック＝0）
                int clicks = tickIndex;
                int digit = (clicks == 10) ? 0 : Mathf.Clamp(clicks, 0, 9);
                string label = digit.ToString();

                Vector3 labelPos = origin.TransformPoint(localDir * (radius + 12f));
                UnityEditor.Handles.Label(labelPos, label, GetLabelStyle());
            }
            tickIndex++;
        }
    }

    static float GetPrivateFloat(object obj, string field, float fallback)
    {
        var fi = obj.GetType().GetField(field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fi != null && fi.FieldType == typeof(float))
        {
            return (float)fi.GetValue(obj);
        }
        return fallback;
    }

    static GUIStyle GetLabelStyle()
    {
        var s = new GUIStyle(UnityEditor.EditorStyles.label);
        s.normal.textColor = Color.white;
        s.fontStyle = FontStyle.Bold;
        return s;
    }

    static void DrawCircle(RectTransform origin, Vector3 center, float radius, int segments = 96)
    {
        Vector3[] pts = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float ang = t * 360f;
            Vector3 local = Quaternion.Euler(0f, 0f, ang) * Vector3.right * radius;
            pts[i] = origin.TransformPoint(local);
        }
        UnityEditor.Handles.DrawAAPolyLine(2f, pts);
    }
#endif
}
*/