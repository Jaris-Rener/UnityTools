using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioCollection))]
public class AudioCollectionInspector : Editor
{
    private AudioCollection _collection;

    public override void OnInspectorGUI()
    {
        _collection = (AudioCollection)target;
        foreach (var bundle in _collection.Collection.ToArray())
        {
            DrawBundle(bundle);
        }

        if (GUILayout.Button("+"))
            AddBundle();
    }

    private void AddBundle()
    {
        _collection.Collection.Add(new AudioBundle());
    }

    private void DrawBundle(AudioBundle bundle)
    {
        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        bundle.Key = EditorGUILayout.TextField(bundle.Key);
        bundle.Clip = (AudioClip)EditorGUILayout.ObjectField(bundle.Clip, typeof(AudioClip), false);
        if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("OL Minus"))))
            _collection.Collection.Remove(bundle);

        if(EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(_collection);

        GUILayout.EndHorizontal();
    }
}
