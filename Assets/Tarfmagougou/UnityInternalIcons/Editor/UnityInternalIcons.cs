/*
 *	Created by Philippe Groarke on 2016-08-28.
 *	Copyright (c) 2016 Tarfmagougou Games. All rights reserved.
 *
 *	Dedication : I dedicate this code to Gabriel, who makes kickass extensions. Now go out and use awesome icons!
 */

using System.IO;
using System.Linq;

namespace tarfmagougou
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    struct BuiltinIcon : System.IEquatable<BuiltinIcon>, System.IComparable<BuiltinIcon>
    {
        public GUIContent icon;
        public GUIContent name;

        public override bool Equals(object o)
        {
            return o is BuiltinIcon && this.Equals((BuiltinIcon)o);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public bool Equals(BuiltinIcon o)
        {
            return this.name.text == o.name.text;
        }

        public int CompareTo(BuiltinIcon o)
        {
            return this.name.text.CompareTo(o.name.text);
        }
    }

    public class UnityInternalIcons : EditorWindow
    {
        List<BuiltinIcon> _icons = new List<BuiltinIcon>();
        Vector2 _scroll_pos;
        GUIContent _refresh_button;
        private string _search;

        [MenuItem("Window/Unity Internal Icons")]
        public static void ShowWindow()
        {
            UnityInternalIcons w = EditorWindow.GetWindow<UnityInternalIcons>();
            TarfmagougouHelperUII.SetWindowTitle(w, "Internal Icons");
        }

        void OnEnable()
        {
            _refresh_button = new GUIContent(EditorGUIUtility.IconContent("d_preAudioLoopOff").image,
                "Refresh : Icons are only loaded in memory when the appropriate window is opened.");

            FindIcons();
        }

        /* Find all textures and filter them to narrow the search. */
        void FindIcons()
        {
            _icons.Clear();

            Texture[] t = Resources.FindObjectsOfTypeAll<Texture>();
            foreach (Texture x in t)
            {
                if (x.name.Length == 0)
                    continue;

                if (x.hideFlags != HideFlags.HideAndDontSave && x.hideFlags != (HideFlags.HideInInspector | HideFlags.HideAndDontSave))
                    continue;

                if (!EditorUtility.IsPersistent(x))
                    continue;

                /* This is the *only* way I have found to confirm the icons are indeed unity builtin. Unfortunately
				 * it uses LogError instead of LogWarning or throwing an Exception I can catch. So make it shut up. */
                TarfmagougouHelperUII.DisableLogging();
                GUIContent gc = EditorGUIUtility.IconContent(x.name);
                TarfmagougouHelperUII.EnableLogging();

                if (gc == null)
                    continue;
                if (gc.image == null)
                    continue;

                _icons.Add(new BuiltinIcon()
                {
                    icon = gc,
                    name = new GUIContent(x.name)
                });
            }

            _icons.Sort();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            Repaint();
        }

        void OnGUI()
        {
            _search = EditorGUILayout.TextField(_search);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button(_refresh_button, EditorStyles.toolbarButton))
            {
                FindIcons();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Found " + _icons.Count + " icons");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Double-click name to copy", TarfmagougouHelperUII.GetMiniGreyLabelStyle());

            _scroll_pos = EditorGUILayout.BeginScrollView(_scroll_pos);
            EditorGUILayout.Space();

            var filteredIcons = string.IsNullOrEmpty(_search)
                ? _icons
                : _icons.Where(x => x.name.text.Contains(_search)).ToList();


            EditorGUIUtility.labelWidth = 100;
            for (int i = 0; i < filteredIcons.Count; ++i)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent(EditorGUIUtility.FindTexture("SceneLoadIn")), (GUIStyle)"RL FooterButton", GUILayout.Width(24)))
                {
                    var savePath = EditorUtility.SaveFilePanel("Save Icon", Application.dataPath, filteredIcons[i].name.text, "png");
                    if (!string.IsNullOrEmpty(savePath))
                    {
                        SaveTexture(savePath, (Texture2D)filteredIcons[i].icon.image);
                    }
                }
                EditorGUILayout.LabelField(filteredIcons[i].icon, filteredIcons[i].name);
                GUILayout.EndHorizontal();

                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.clickCount > 1)
                {
                    EditorGUIUtility.systemCopyBuffer = filteredIcons[i].name.text;
                    Debug.Log(filteredIcons[i].name.text + " copied to clipboard.");
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void SaveTexture(string path, Texture2D texture)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(
                texture.width,
                texture.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(texture, tmp);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D myTexture2D = new Texture2D(texture.width, texture.height);
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            var bytes = (myTexture2D).EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            Debug.Log($"{texture.name} saved to {path}");
        }
    }
}
