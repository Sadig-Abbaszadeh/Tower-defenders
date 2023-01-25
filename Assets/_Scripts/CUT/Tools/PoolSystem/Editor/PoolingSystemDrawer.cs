using UnityEditor;
using UnityEngine;

namespace DartsGames.CUT.Editors
{
    [CustomPropertyDrawer(typeof(PoolingParams), true)]
    public class PoolingParamsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            label.tooltip = "PoolObject to be pooled";

            Rect pos;

            if (position.height > 16)
            {
                pos = EditorGUI.PrefixLabel(position, label);
                EditorGUI.indentLevel = 0;

                pos.y += 18;
                pos.height = 16;
                pos.x = 35;
                pos.width *= 1.2f;
            }
            else
            {
                label.text = "El." + label.text[label.text.Length - 1];
                pos = EditorGUI.PrefixLabel(position, label);
                EditorGUI.indentLevel = 0;

                pos.x = 60;
            }

            pos.width *= .4f;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("obj"), GUIContent.none);

            pos.x += pos.width + 8;
            pos.width *= 1.1f;
            EditorGUIUtility.labelWidth = 10f;

            var st = property.serializedObject.FindProperty("type");
            var pt = property.FindPropertyRelative("objType");

            st.intValue = pt.intValue;

            EditorGUI.PropertyField(pos, st, new GUIContent("T", "Enum type of this poolobject"));

            pt.intValue = st.intValue;

            pos.x += pos.width + 2;
            pos.width *= 1.2f;
            EditorGUIUtility.labelWidth = 35f;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("initialCount"), new GUIContent("count", "Amount that will be pre-instantiated"));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => Screen.width < 350 ? 34 : 16;
    }
}