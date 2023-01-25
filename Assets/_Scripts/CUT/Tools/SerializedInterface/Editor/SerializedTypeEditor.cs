using DartsGames.CUT.EditorExtensions;
using System;
using UnityEditor;
using UnityEngine;
using DartsGames.CUT.UnityExtensions;

namespace DartsGames.CUT.Editors
{
    [CustomPropertyDrawer(typeof(SerializedType<>), true)]
    public class SerializedTypeEditor : PropertyDrawer
    {
        private bool initialized = false;
        private Type t;
        private SerializedObject so;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!initialized)
                Initialize(property);

            label.text += " : " + t.Name;
            Rect newPos = EditorGUI.PrefixLabel(EditorGUI.IndentedRect(position), label);

            var c = property.FindPropertyRelative("c");
            var val = c.objectReferenceValue;

            newPos.height = 18;
            EditorGUI.PropertyField(newPos, c, GUIContent.none);

            // change occured
            if (val != c.objectReferenceValue && c.objectReferenceValue != null)
            {
                if (!t.IsAssignableFrom(c.objectReferenceValue.GetType()))
                {
                    var comp = ((Component)c.objectReferenceValue).GetComponent(t);
                    c.objectReferenceValue = comp;
                }

                so.ApplyModifiedProperties();
            }
        }

        private void Initialize(SerializedProperty serP)
        {
            initialized = true;

            var info = serP.GetFieldInformation();

            t = info.PropertyType.GenericTypeArguments[0];
            so = serP.serializedObject;

            // check field's periouvsly serialized value
            var c = serP.FindPropertyRelative("c");
            var val = c.objectReferenceValue;

            if (val != null && !t.IsAssignableFrom(val.GetType()))/*val.GetType() != t)*/
            {
                Debug.Log("shifted");
                c.objectReferenceValue = null;
            }
        }
    }
}