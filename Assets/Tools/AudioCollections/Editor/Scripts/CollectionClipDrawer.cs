namespace AudioCollections
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(CollectionClip))]
    public class CollectionClipDrawer
        : PropertyDrawer
    {
        private const int _width = 128;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * (property.FindPropertyRelative("_useCollection").boolValue ? 2 : 1);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.BeginChangeCheck();
            property.FindPropertyRelative("_useCollection").boolValue = GUI.Toggle(new Rect(position) { width = _width },
                property.FindPropertyRelative("_useCollection").boolValue, "Use Collection", "Button");

            if (property.FindPropertyRelative("_useCollection").boolValue)
            {
                EditorGUI.PropertyField(new Rect(position.x + _width, position.y, position.width - _width, position.height),
                    property.FindPropertyRelative("_collection"), GUIContent.none);
                AudioCollection collection =
                    property.FindPropertyRelative("_collection").objectReferenceValue as AudioCollection;

                position.position = new Vector2(position.x, position.y + EditorGUIUtility.singleLineHeight);
                if (collection != null)
                {
                    var list = new string[collection.Collection.Count];
                    for (var i = 0; i < collection.Collection.Count; ++i)
                    {
                        list[i] = collection.Collection[i].Key;
                    }

                    property.FindPropertyRelative("_selected").intValue =
                        EditorGUI.Popup(position, property.FindPropertyRelative("_selected").intValue, list);
                }
            }
            else
            {
                EditorGUI.PropertyField(new Rect(position.x + _width, position.y, position.width - _width, position.height),
                    property.FindPropertyRelative("_singleClip"), GUIContent.none, false);
            }


            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }

}