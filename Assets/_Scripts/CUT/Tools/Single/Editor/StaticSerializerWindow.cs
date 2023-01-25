using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using DartsGames;
using System.IO;
using UnityEditor.Callbacks;

namespace DartsGames.CUT
{
    public class StaticSerializerWindow : EditorWindow
    {
        #region Static
        private static StaticSerializerWindow Instance = null;

        [MenuItem("DartsGames/CUT/StaticSerializer")]
        private static void CreateWindow()
        {
            if (Instance != null)
            {
                Instance.Focus();
                return;
            }

            Instance = EditorWindow.CreateWindow<StaticSerializerWindow>("Static Serializer v1.0");
            Instance.Init();
        }

        [OnOpenAssetAttribute(1)]
        public static bool OpenAsset(int instanceID, int line)
        {
            if (Instance == null) return false;

            var obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj is MonoScript ms)
            {
                Instance.ms = ms;
                Instance.Focus();
                Instance.UpdateScriptInfo();
                return true;
            }

            return false;
        }

        #endregion

        private MonoScript ms;
        private SerializedProperty sp;
        private SerializedObject so;

        private List<(FieldInfo, StaticPropertyField)> staticFields = new List<(FieldInfo, StaticPropertyField)>();

        private void Init()
        {
            var so = new SerializedObject(this);
            sp = so.FindProperty("ms");
        }

        private void OnGUI()
        {
            var _newMs = EditorGUILayout.ObjectField("Script", ms, typeof(MonoScript), false);

            if (_newMs != ms)
            {
                ms = _newMs as MonoScript;
                UpdateScriptInfo();
            }

            foreach (var sf in staticFields)
                sf.Item2.Draw();

            if (staticFields.Count > 0 && (GUILayout.Button("Apply changes")))
                ApplyChangesToCode();
        }

        private void ApplyChangesToCode()
        {
            var path = GetMonoScriptPath(ms);
            var text = ms.text;

            foreach (var sf in staticFields)
            {
                var fieldSignature = (GetPrivacy(sf.Item1) + " static " + GetFieldType(sf.Item1) + " " + sf.Item1.Name);

                var fieldStartIndex = text.IndexOf(fieldSignature);
                var fieldLastIndex = text.IndexOf(';', fieldStartIndex);

                var fieldFullText = text.Substring(fieldStartIndex, fieldLastIndex - fieldStartIndex);

                text = text.Replace(fieldFullText, fieldSignature + " = " + sf.Item2.GetValue());
            }

            File.WriteAllText(path, text);
            AssetDatabase.Refresh();
        }

        private void UpdateScriptInfo()
        {
            staticFields.Clear();

            if (ms == null || ms.GetClass() == null)
            {
                // handle incorrect Mono here
                Debug.Log("err)");
                return;
            }

            var path = GetMonoScriptPath(ms);
            var type = ms.GetClass();

            var eligibleFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(f => supportedTypes.ContainsKey(f.FieldType));

            foreach (var f in eligibleFields)
            {
                var propertyField = GetPropertyField(f);

                propertyField.name = f.Name;
                propertyField.SetDefaultValue(f.GetValue(null));

                staticFields.Add((f, propertyField));
            }
        }

        #region Utils

        private static readonly Dictionary<Type, (string, Func<StaticPropertyField>)> supportedTypes = new Dictionary<Type, (string, Func<StaticPropertyField>)>()
    {
        { typeof(int), ("int", () => new IntField()) },
        { typeof(string), ("string", () => new StringField()) },
        { typeof(bool), ("bool", () => new BoolField()) },
        { typeof(float), ("float", () => new FloatField()) },
    };

        private string GetFieldType(FieldInfo f) => supportedTypes[f.FieldType].Item1;

        private string GetPrivacy(FieldInfo f) => f.IsPublic ? "public" : (f.IsPrivate ? "private" : "protected");

        private string GetMonoScriptPath(MonoScript ms) => Application.dataPath + AssetDatabase.GetAssetPath(ms).Substring(6);

        private StaticPropertyField GetPropertyField(FieldInfo f) => supportedTypes[f.FieldType].Item2();

        #endregion

        #region Fields
        public abstract class StaticPropertyField
        {
            public string name;

            public abstract void Draw();
            public abstract string GetValue();
            public abstract void SetDefaultValue(object value);
        }

        public class IntField : StaticPropertyField
        {
            private int value;

            public override void Draw()
            {
                value = EditorGUILayout.IntField(name, value);
            }

            public override string GetValue() => value.ToString();

            public override void SetDefaultValue(object value) => this.value = (int)value;
        }

        public class StringField : StaticPropertyField
        {
            private string value;

            public override void Draw()
            {
                value = EditorGUILayout.TextField(name, value);
            }

            public override string GetValue() => value.Equals("null") ? value
                : "@\"" + value + "\"";

            public override void SetDefaultValue(object value) => this.value = (string)value;
        }

        public class BoolField : StaticPropertyField
        {
            private bool value;

            public override void Draw()
            {
                value = EditorGUILayout.Toggle(name, value);
            }

            public override string GetValue() => value ? "true" : "false";

            public override void SetDefaultValue(object value) => this.value = (bool)value;
        }

        public class FloatField : StaticPropertyField
        {
            private float value;

            public override void Draw()
            {
                value = EditorGUILayout.FloatField(name, value);
            }

            public override string GetValue() => value + "f";

            public override void SetDefaultValue(object value) => this.value = (float)value;
        }

        #endregion
    }
}