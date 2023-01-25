using DartsGames.CUT.Attributes;
using UnityEditor;
using UnityEngine;

namespace DartsGames.CUT.Editors.Drawables
{
    public class Property : Drawable
    {
        public SerializedProperty sp;

        // ctor
        public Property(SerializedProperty sp, InspectConditionAttribute conditionAttr, RequiredAttribute requiredAttr)
            : base(sp.serializedObject.targetObject, sp, conditionAttr, requiredAttr)
        {
            this.sp = sp;
        }

        public override void Draw(float width)
        {
            if (validation())
                Draw_Confirmed(width);
        }

        protected virtual void Draw_Confirmed(float width)
        {
            Draw_Self(width);
            drawExtras(width);
        }

        protected void Draw_Self(float width)
        {
            EditorGUILayout.PropertyField(sp, GUILayout.Width(width));
        }
    }
}