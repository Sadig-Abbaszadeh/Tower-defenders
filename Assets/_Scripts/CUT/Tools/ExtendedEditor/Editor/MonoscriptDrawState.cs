using UnityEditor;
using UnityEngine;

namespace DartsGames.CUT.Editors
{
    public class MonoscriptDrawState : IEditorState
    {
        private MonoScript script;

        public virtual void Disable() { }

        public virtual void OnGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            GUI.enabled = true;
        }

        // ctor
        public MonoscriptDrawState(object obj)
        {
            if (obj is MonoBehaviour mb)
                script = MonoScript.FromMonoBehaviour(mb);
            else
                script = MonoScript.FromScriptableObject(obj as ScriptableObject);
        }
    }
}