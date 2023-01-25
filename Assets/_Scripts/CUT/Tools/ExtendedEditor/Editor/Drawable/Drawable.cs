using DartsGames.CUT.Attributes;
using DartsGames.CUT.CoreExtensions;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DartsGames.CUT.Editors.Drawables
{
    public abstract class Drawable
    {
        public int order = 0;

        protected Func<bool> validation = () => true;
        protected Action<float> drawExtras = f => { };

        // default ctor
        public Drawable() { }

        // ctor
        public Drawable(UnityEngine.Object contextObj, InspectConditionAttribute conditionAttr)
        {
            if (conditionAttr == null || string.IsNullOrEmpty(conditionAttr.MemberName)) return;

            var t = contextObj.GetType();

            var method = t.GetMethodInAncestors(conditionAttr.MemberName);

            if (method != null && method.GetParameters().Length == 0 && method.ReturnType != typeof(void))
            {
                if (conditionAttr.Reverse)
                    validation = () => !method.Invoke(contextObj, null).Equals(conditionAttr.Value);
                else
                    validation = () => method.Invoke(contextObj, null).Equals(conditionAttr.Value);
                return;
            }

            var field = t.GetFieldInAncestors(conditionAttr.MemberName);

            if (field != null)
            {
                if (conditionAttr.Reverse)
                    validation = () => !field.GetValue(contextObj).Equals(conditionAttr.Value);
                else
                    validation = () => field.GetValue(contextObj).Equals(conditionAttr.Value);
            }
        }

        // ctor
        public Drawable(UnityEngine.Object contextObj, SerializedProperty sp,
            InspectConditionAttribute conditionAttr, RequiredAttribute requiredAttr) : this(contextObj, conditionAttr)
        {
            if (sp.propertyType != SerializedPropertyType.ObjectReference || requiredAttr == null) return;

            var type = (MessageType)requiredAttr.messageType;
            var message = requiredAttr.message;

            if (string.IsNullOrEmpty(message))
                message = "This object is required!";

            drawExtras += width =>
            {
                if (sp.objectReferenceValue == null)
                    EditorGUILayout.HelpBox(message, type);
            };
        }

        public abstract void Draw(float width);

        public static void AddByOrder(List<Drawable> list, Drawable item)
        {
            var index = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (item.order < list[i].order)
                {
                    list.Insert(index, item);
                    return;
                }
            }

            list.Add(item);
        }
    }
}