using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DartsGames.CUT.Editors.Drawables
{
    public class Box : Drawable
    {
        protected List<Drawable> contents = new List<Drawable>();

        public void AddElement(Drawable d)
        {
            AddByOrder(contents, d);
        }

        public override void Draw(float width)
        {
            EditorGUILayout.BeginHorizontal();

            var w = width / contents.Count;
            var initLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = w * .4f;

            foreach (var d in contents)
            {
                d.Draw(w);
            }

            EditorGUIUtility.labelWidth = initLabelWidth;

            EditorGUILayout.EndHorizontal();
        }
    }
}