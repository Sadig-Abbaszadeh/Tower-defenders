using DartsGames.CUT.Attributes;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DartsGames.CUT.Editors
{
    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    //[CustomEditor(typeof(ScriptableObject), true)]
    public class ExtendedEditor : Editor, IEditorState
    {
        protected IEditorState state;

        protected virtual void OnEnable()
        {
            var t = target.GetType();

            //

            // is extended editor
            if (Attribute.IsDefined(t, typeof(ExtendEditorAttribute)))
            {
                //if (Attribute.IsDefined(t, typeof(DesignerComponentAttribute)))
                //{
                //    DesignerObjectsHandler.OnDesignerModeChanged += DesignerModeChanged;
                //    state = new ExtendedState(target, serializedObject, DesignerFieldCheck, DesignerMethodCheck);
                //}
                //else
                {
                    state = new ExtendedState(target, serializedObject, f => true, m => true);
                }
            }
            // is normal editor
            else
            {
                state = this;
            }
        }

        private void DesignerModeChanged()
        {
            if (state is ExtendedState extendedState)
            {
                extendedState.Refresh(target, serializedObject, DesignerFieldCheck, DesignerMethodCheck);
                Repaint();
            }
        }

        protected virtual void OnDisable()
        {
            //DesignerObjectsHandler.OnDesignerModeChanged -= DesignerModeChanged;
            state.Disable();
        }

        public sealed override void OnInspectorGUI() => state.OnGUI();

        #region Default inspector
        /// <summary>
        /// Override this instead of OnInspectorGUI if you want to override the default inspector gui
        /// </summary>
        public virtual void OnGUI()
        {
            base.OnInspectorGUI();
        }

        /// <summary>
        /// If state is regular state, this is called on disable, else the state.disable will be called on the state. Thus only override this. OnEnable is not changed for now
        /// </summary>
        public virtual void Disable() { }
        #endregion

        private bool DesignerFieldCheck(FieldInfo fieldInfo) => true;//!DesignerObjectsHandler.DesignerMode || Attribute.IsDefined(fieldInfo, typeof(DesignerFieldAttribute));
        private bool DesignerMethodCheck(MethodInfo methodInfo) => true;//!DesignerObjectsHandler.DesignerMode || Attribute.IsDefined(methodInfo, typeof(DesignerMethodAttribute));
    }
}