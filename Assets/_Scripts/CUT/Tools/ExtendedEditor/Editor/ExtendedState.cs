using DartsGames.CUT.Attributes;
using DartsGames.CUT.CoreExtensions;
using DartsGames.CUT.Editors.Drawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DartsGames.CUT.Editors
{
    public class ExtendedState : MonoscriptDrawState
    {
        #region Static
        private static int boxRightOffset = 25;
        #endregion

        // serialized object
        private SerializedObject so;

        // reg props
        private List<Drawable> drawables = new List<Drawable>();

        private float width;

        // drawer method
        public override void OnGUI()
        {
            width = Screen.width - boxRightOffset;

            base.OnGUI();

            EditorGUI.BeginChangeCheck();

            DrawRegular(width);

            if (!EditorGUI.EndChangeCheck())
                return;

            EditorUtility.SetDirty(so.targetObject);
            so.ApplyModifiedProperties();
            so.UpdateIfRequiredOrScript();
        }

        public override void Disable()
        {
            Undo.undoRedoPerformed -= SoUpdate;
        }

        // ctor
        /// <summary>
        /// Dont forget to call ondisable on the object on disable
        /// </summary>
        public ExtendedState(UnityEngine.Object target, SerializedObject obj, Func<FieldInfo, bool> CanSerializeField, Func<MethodInfo, bool> CanSerializeMethod) : base(target)
        {
            PrepareMain(target, obj, CanSerializeField, CanSerializeMethod);
            Undo.undoRedoPerformed += SoUpdate;
        }

        public void Refresh(UnityEngine.Object target, SerializedObject obj, Func<FieldInfo, bool> CanSerializeField, Func<MethodInfo, bool> CanSerializeMethod)
        {
            drawables.Clear();

            PrepareMain(target, obj, CanSerializeField, CanSerializeMethod);
        }

        private void DrawRegular(float widith)
        {
            foreach (var sp in drawables)
                sp.Draw(width);
        }

        private void SoUpdate()
        {
            so.ApplyModifiedProperties();
            so.Update();
        }

        #region Preparation
        private void PrepareMain(UnityEngine.Object target, SerializedObject obj, Func<FieldInfo, bool> CanSerializeField, Func<MethodInfo, bool> CanSerializeMethod)
        {
            so = obj;
            var targetType = target.GetType();

            var restrTypes = new List<Type>() { typeof(MonoBehaviour), typeof(ScriptableObject) };
            var allMethods = targetType.GetAllMethodsInAncestors(restrTypes);
            var allFields = targetType.GetAllFieldsInAncestors(restrTypes);

            // all serializable fields
            var serializableFields = allFields.Where(f => so.FindProperty(f.Name) != null &&
                !Attribute.IsDefined(f, typeof(HideInInspector)) &&
                CanSerializeField(f));

            // all button methods
            var usableMethods = allMethods.Where(m => m.GetParameters().Length == 0 &&
                Attribute.IsDefined(m, typeof(InspectorButtonAttribute)) && CanSerializeMethod(m));

            var boxes = new Dictionary<string, Box>();
            var tabs = new Dictionary<string, Tab>();

            // set up fields
            foreach (var sf in serializableFields)
            {
                var inline = sf.GetCustomAttribute<InlineEditorAttribute>();
                var sp = so.FindProperty(sf.Name);

                var condAttr = sf.GetCustomAttribute<InspectConditionAttribute>();
                var requiredAttr = sf.GetCustomAttribute<RequiredAttribute>();

                if (sp.propertyType == SerializedPropertyType.ObjectReference && inline != null)
                {
                    drawables.Add(
                        new InlineProperty(sp, condAttr, requiredAttr));

                    continue;
                }

                var prop = new Property(sp, condAttr, requiredAttr);

                var result = SetDrawableBox(sf, boxes, drawables, prop);

                if(TryGetTab(sf, tabs, out var tab) && result != null)
                {
                    tab.contents.Add(result);
                }
            }

            foreach (var m in usableMethods)
            {
                var button = new Button(m, target, m.GetCustomAttribute<InspectConditionAttribute>());

                var result = SetDrawableBox(m, boxes, drawables, button);

                if(TryGetTab(m, tabs, out var tab) && result != null)
                {
                    tab.contents.Add(result);
                }
            }

            // check if there are enough tabs
            if(tabs.Count > 0)
            {
                CollectTabs(tabs);
            }
        }

        private void CollectTabs(Dictionary<string, Tab> tabDict)
        {
            var tabs = tabDict.ValuesList();

            // clear empty tabs resulting from box-tab contradiction
            for (int i = 0; i < tabs.Count; i++)
            {
                if (tabs[i].contents.Count == 0)
                {
                    tabs.RemoveAt(i);
                    i--;
                }
            }

            var defTab = new Tab("Default");
            defTab.activated = true;
            defTab.contents = new List<Drawable>(drawables);

            drawables.Clear();

            tabs.Insert(0, defTab);

            foreach (var t in tabs)
            {
                if (t.contents.Count == 0) continue;

                drawables.Add(t);
                t.allTabs = tabs;
            }

            tabs[0].activated = true;
        }

        // returns true and tab when the member can be added into a tab
        private bool TryGetTab(MemberInfo m, Dictionary<string, Tab> allTabs, out Tab tab)
        {
            var tabAttr = m.GetCustomAttribute<TabGroupAttribute>();

            if (tabAttr == null || tabAttr.tabName == null)
            {
                tab = default;
                return false;
            }

            if(allTabs.TryGetValue(tabAttr.tabName, out tab))
            {
                return true;
            }

            tab = new Tab(tabAttr.tabName);
            allTabs.Add(tabAttr.tabName, tab);
            return true;
        }

        /// <summary>
        /// Returns Drawable: 1) when new box is created, ret box, 2) when box is null ret d
        /// Returns null when adding to existing box
        /// </summary>
        private Drawable SetDrawableBox(MemberInfo m, Dictionary<string, Box> boxes, List<Drawable> defaultColl, Drawable d)
        {
            var area = m.GetCustomAttribute<HorizontalAreaAttribute>();

            if (area == null)
            {
                defaultColl.Add(d);
                return d;
            }

            // box doesnt exist
            if (!boxes.TryGetValue(area.Name, out var box))
            {
                box = new Box();
                boxes.Add(area.Name, box);
                defaultColl.Add(box);
                box.AddElement(d);
                return box;
            }

            box.AddElement(d);
            return null;
        }
        #endregion
    }
}