using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace HierarchyExtensions
{
    [InitializeOnLoad]
    public class HierarchyFormat
    {
        private static GUIStyle _labelStyle;
        private static string[] _visibleObjs;

        [InitializeOnLoadMethod]
        public static void Init()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItem_CB;
        }

        private static void GetVisibleObjects()
        {
            // Holy heck this method really sucks - I could not find a callback or anything like onHierarchyItemExpanded.
            var window = Resources.FindObjectsOfTypeAll<EditorWindow>().First(x => x.GetType().Name == "SceneHierarchyWindow");
            var type = window.GetType();
            var method = type.GetMethod("GetCurrentVisibleObjects", BindingFlags.Public | BindingFlags.Instance);
            var field = type.GetField("m_SceneHierarchy", BindingFlags.NonPublic | BindingFlags.Instance);
            var objs = (string[])method?.Invoke(window, null);
            _visibleObjs = objs;
        }

        private static void HierarchyWindowItem_CB(int id, Rect rect)
        {
            if(_labelStyle == null)
                _labelStyle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };

            var obj = EditorUtility.InstanceIDToObject(id);
            var go = obj as GameObject;

            if(go == null)
                return;

            var divider = go.GetComponent<EditorDivider>();

            if (divider == null)
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            bool selected = Selection.objects.Contains(obj);

            var backgroundRect = new Rect(rect);
            backgroundRect.x = 32f;
            backgroundRect.width += 512f;
            backgroundRect.height -= 2;
            backgroundRect.y += 1f;
            GUI.backgroundColor = divider.Color;
            GUI.Box(backgroundRect, GUIContent.none, "OverrideMargin");

            if (selected)
            {
                GUI.backgroundColor = new Color(0f, 0.49f, 1f, 0.5f);
                GUI.Box(backgroundRect, GUIContent.none, "OverrideMargin");
            }

            GUI.backgroundColor = Color.white;

            var iconRect = new Rect(rect);
            iconRect.width = 20;
            iconRect.height = 20;
            iconRect.x += -2f;
            iconRect.y += -2f;
            var colorFactor = 0.35f;
            GUI.color = new Color(divider.Color.r * colorFactor, divider.Color.g * colorFactor, divider.Color.b * colorFactor);
            GUI.Label(iconRect, EditorGUIUtility.IconContent("d_Favorite"));
            GUI.color = Color.white;

            var labelRect = new Rect(rect);
            labelRect.y -= 1f;
            labelRect.x += 17f;
            labelRect.height += 2f;
            GUI.contentColor = new Color(divider.Color.r*colorFactor, divider.Color.g* colorFactor, divider.Color.b* colorFactor);
            GUI.Label(labelRect, divider.name, _labelStyle);
            GUI.contentColor = Color.white;

            if (go.transform.childCount > 0)
            {
                GetVisibleObjects();

                var expanded = !_visibleObjs.Contains(go.transform.GetChild(0).name);
                var foldoutRect = new Rect(rect);
                foldoutRect.x -= 16f;
                GUI.color = new Color(divider.Color.r * colorFactor, divider.Color.g * colorFactor, divider.Color.b * colorFactor);
                GUI.Label(foldoutRect, EditorGUIUtility.IconContent(expanded ? "IN foldout act" : "IN foldout act on"));
                GUI.color = Color.white;
            }

            EditorApplication.RepaintHierarchyWindow();
        }
    }
}