using DartsGames.CUT.Attributes;
using System;
using System.Reflection;
using UnityEngine;

namespace DartsGames.CUT.Editors.Drawables
{
    public class Button : Drawable
    {
        private Action action;
        private GUIContent name;

        // ctor
        public Button(MethodInfo method, UnityEngine.Object target, InspectConditionAttribute inspectCondition)
            : base(target, inspectCondition)
        {
            this.action = () => method.Invoke(target, null);
            name = new GUIContent(method.Name);
        }

        public override void Draw(float width)
        {
            if (validation() && GUILayout.Button(name))
                action();
        }
    }
}