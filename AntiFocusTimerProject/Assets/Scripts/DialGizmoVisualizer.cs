// �t�@�C�����FDialGizmoVisualizer.cs
// �ړI�FDialController �� clickStepDegrees / minAngle / maxAngle �Ɋ�Â���
//       Scene �r���[��Ɂu�N���b�N���E�i���p�x�̖ڐ���j�v��\������B
// �g�����FDialController �Ɠ��� GameObject �ɕt���A�K�v�Ȃ甼�a��F�𒲐��B
/*
using UnityEngine;

[ExecuteAlways]
public class DialGizmoVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] DialController dialController;   // ����GO����
    [SerializeField] RectTransform dialOrigin;        // �p�x�̊�i�ʏ� DialRotator�j
                                                      // ���ݒ�Ȃ� dialController �� angleOrigin or dialRotator �������g�p

    [Header("Appearance")]
    [SerializeField, Tooltip("�~���̉������a�iUI�s�N�Z�������j�B���ݒ莞��Rect�̒Z��/2���g�p")]
    float radiusOverride = 0f;
    [SerializeField, Tooltip("�ڐ�����̒����i�������j")]
    float tickLength = 20f;
    [SerializeField] Color rimColor = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] Color tickColor = new Color(1f, 0.65f, 0f, 0.9f);
    [SerializeField] bool showLabels = true;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (dialController == null) dialController = GetComponent<DialController>();
        if (dialController == null) return;

        // �Q�Ɖ���
        RectTransform origin = dialOrigin;
        if (origin == null)
        {
            // DialController ���� angleOrigin �܂��� dialRotator �𗬗p
            var angleOriginField = typeof(DialController).GetField("angleOrigin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dialRotatorField = typeof(DialController).GetField("dialRotator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            origin = (RectTransform)(angleOriginField?.GetValue(dialController) as RectTransform)
                     ?? (RectTransform)(dialRotatorField?.GetValue(dialController) as RectTransform);
        }
        if (origin == null) return;

        // �p�x�p�����[�^�擾
        float step = GetPrivateFloat(dialController, "clickStepDegrees", 12f);
        float minAngle = GetPrivateFloat(dialController, "minAngle", -120f);
        float maxAngle = GetPrivateFloat(dialController, "maxAngle", 0f);
        float zeroOff = GetPrivateFloat(dialController, "zeroOffsetDeg", 45f); // �� �ǉ�
        if (step <= 0.0001f) return;

        // ���a����iUI�z��FRect �̒Z��/2�j
        float radius = radiusOverride > 0f ? radiusOverride : Mathf.Min(origin.rect.width, origin.rect.height) * 0.5f;

        // ���_�i���[���h�j
        Vector3 center = origin.TransformPoint(Vector3.zero);

        // �~���i�����j�𔖂��`��
        UnityEditor.Handles.color = rimColor;
        DrawCircle(origin, center, radius);

        // �ڐ���i�N���b�N���E�j��`��FmaxAngle(�ʏ�0��) �� minAngle(��) �܂� step ����
        UnityEditor.Handles.color = tickColor;

        int tickIndex = 0;
        for (float a = maxAngle; a >= minAngle - 0.001f; a -= step)
        {
            // �� �g�^�ぁ0���h�̃��W�b�N�p a �� zeroOffset �𑫂��Č����ڊp��
            Vector3 localDir = Quaternion.Euler(0f, 0f, zeroOff + a) * Vector3.right;

            Vector3 worldOuter = origin.TransformPoint(localDir * radius);
            Vector3 worldInner = origin.TransformPoint(localDir * (radius - tickLength));
            UnityEditor.Handles.DrawAAPolyLine(2f, new Vector3[] { worldOuter, worldInner });

            if (showLabels)
            {
                // �� �N���b�N�����d�b�̐����i10�N���b�N��0�j
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