using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace EditorUtilities
{
    public class SceneChangerOverlay
        : EditorWindow
    {
        private enum LoadMode
        {
            Multiple,
            Single
        }

        private static bool _visible;
        private static Vector2 _windowPos;
        private static Vector2 _offset;
        private static SceneView _sceneView;
        private static LoadMode _mode;

        [InitializeOnLoadMethod]
        [MenuItem("Tools/Scene Switcher %&s")]
        private static void Enable()
        {
            if (_visible)
            {
                Disable();
                return;
            }

            SceneView.duringSceneGui += OnSceneGui;
            _visible = true;
        }

        private static void Disable()
        {
            SceneView.duringSceneGui -= OnSceneGui;
            _visible = false;
        }

        private static void OnSceneGui(SceneView sceneView)
        {
            _mode = Event.current.shift ? LoadMode.Multiple : LoadMode.Single;

            _sceneView = sceneView;
            _sceneView.wantsMouseMove = true;
            if (Event.current.type == EventType.MouseMove)
                _sceneView.Repaint();

            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(Vector2.zero, _sceneView.maxSize));
            GUILayout.BeginVertical();
            GUILayout.Space(16);
            var activeStyle = new GUIStyle((GUIStyle) "LargeButtonRight");
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                var activeScene = SceneManager.GetSceneByPath(scenePath).isLoaded;

                if (!activeScene)
                    GUI.backgroundColor = new Color(0.75f, 0.75f, 0.75f);

                if (GUILayout.Button(sceneName, activeStyle, GUILayout.MaxWidth(125), GUILayout.Height(24)))
                {
                    if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        return;

                    switch (_mode)
                    {
                        case LoadMode.Multiple:
                            if (activeScene)
                                EditorSceneManager.CloseScene(SceneManager.GetSceneByPath(scenePath), true);
                            else
                                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                            break;
                        case LoadMode.Single:
                            EditorSceneManager.OpenScene(scenePath);
                            break;
                    }
                }

                GUI.backgroundColor = Color.white;

                var button = GUILayoutUtility.GetLastRect();
                button.width = 20;
                GUI.Toggle(button, activeScene, GUIContent.none);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
            Handles.EndGUI();
        }
    }
}