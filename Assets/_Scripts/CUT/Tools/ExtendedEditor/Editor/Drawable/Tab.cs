using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DartsGames.CUT.Editors.Drawables
{
    public class Tab : Drawable
    {
        private GUIContent name;
        public List<Drawable> contents = new List<Drawable>();

        public List<Tab> allTabs = new List<Tab>();

        public bool activated = false;

        public Tab(string tabName)
        {
            name = new GUIContent(tabName);
        }

        public override void Draw(float width)
        {
            if (!activated) return;
            
            EditorGUILayout.BeginHorizontal();

            //var w = width / contents.Count;
            //var initLabelWidth = EditorGUIUtility.labelWidth;
            //EditorGUIUtility.labelWidth = w * .4f;

            foreach(var t in allTabs)
            {
                GUI.enabled = !t.activated;

                if(GUILayout.Button(t.name))
                {
                    this.activated = false;

                    t.activated = true;
                }

                GUI.enabled = true;
            }

            //EditorGUIUtility.labelWidth = initLabelWidth;
            EditorGUILayout.EndHorizontal();

            foreach (var d in contents)
                d.Draw(width);
        }
    }
}