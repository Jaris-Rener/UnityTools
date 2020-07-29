using UnityEngine;

namespace HierarchyExtensions
{
    using UnityEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(EditorDivider))]
    public class EditorDividerInspector
        : Editor
    {
        private static readonly Color[] _palette =
        {
            new Color(0.101f, 0.737f, 0.611f),
            new Color(0.180f, 0.800f, 0.443f),
            new Color(0.203f, 0.596f, 0.858f),
            new Color(0.607f, 0.349f, 0.713f),
            new Color(0.203f, 0.286f, 0.368f),
            new Color(0.945f, 0.768f, 0.058f),
            new Color(0.901f, 0.494f, 0.133f),
            new Color(0.905f, 0.298f, 0.235f),
            new Color(0.925f, 0.941f, 0.945f),
            new Color(0.584f, 0.647f, 0.650f),
        };

        public override void OnInspectorGUI()
        {
            var divider = (EditorDivider) target;

            EditorGUI.BeginChangeCheck();
            var color = EditorGUILayout.ColorField(divider.Color);

            var rect = GUILayoutUtility.GetLastRect();
            var w = rect.width;
            rect.y = rect.yMax + 2;
            rect.height = 16;
            ++rect.x;
            rect.width = w/_palette.Length - 3;
            for (var i = 0; i < _palette.Length; i++)
            {
                var item = _palette[i];
                GUI.backgroundColor = item;
                if (GUI.Button(rect, GUIContent.none, (GUIStyle)"WhiteBackground"))
                    color = item;

                GUI.backgroundColor = Color.white;
                rect.x = rect.xMax + 1;
            }
            GUILayout.Space(20);

            if (EditorGUI.EndChangeCheck())
            {
                foreach (var t in targets)
                {
                    Undo.RecordObjects(targets, "Change Divider Color");
                    ((EditorDivider) t).Color = color;
                    EditorUtility.SetDirty(t);
                }
            }
        }
    }
}