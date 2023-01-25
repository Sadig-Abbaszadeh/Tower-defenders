using DartsGames.CUT.Attributes;
using UnityEditor;
using UnityEngine;

namespace DartsGames.CUT.Editors.Drawables
{
    public class InlineProperty : Property
    {
        public static readonly Texture2D defTex = MakeTex(1, 1, new Color(
            .4f, .4f, .4f, 1f));

        protected SerializedProperty sp;
        protected Editor editor;
        protected GUIStyle bckgStyle;
        protected GUIContent foldoutContent;

        protected bool showing = true, available = false;

        private static Texture2D MakeTex(int w, int h, Color col)
        {
            var pix = new Color[w * h];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            var tex = new Texture2D(w, h);
            tex.SetPixels(pix);
            tex.Apply();

            return tex;
        }

        // ctor
        public InlineProperty(SerializedProperty sp, InspectConditionAttribute inspectCondition, RequiredAttribute requiredAttr)
            :base(sp, inspectCondition, requiredAttr)
        {
            this.sp = sp;
            this.foldoutContent = new GUIContent(sp.displayName + " editor");
            this.bckgStyle = new GUIStyle();
            bckgStyle.normal.background = defTex;

            CheckRefreshEditor();
        }

        private void CheckRefreshEditor()
        {
            var obj = sp.objectReferenceValue;
            available = obj != null;

            if (available)
            {
                editor = Editor.CreateEditor(obj);
            }
            else
            {
                editor = null;
            }
        }

        protected override void Draw_Confirmed(float width)
        {
            EditorGUI.BeginChangeCheck();

            base.Draw_Confirmed(width);

            if (EditorGUI.EndChangeCheck())
            {
                CheckRefreshEditor();
            }

            if (!available) return;

            EditorGUILayout.BeginVertical(bckgStyle);
            EditorGUI.indentLevel++;

            showing = EditorGUILayout.Foldout(showing, foldoutContent);

            if (showing)
            {
                editor.OnInspectorGUI();
            }

            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel--;
        }
    }
}