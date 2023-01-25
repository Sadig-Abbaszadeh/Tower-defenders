using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using DartsGames.Extensions;
using System.Linq;
using DartsGames.CUT.EditorExtensions;
using DartsGames.CUT.CoreExtensions;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(PolymorphEditorAttribute))]
public class PolymorphEditor : PropertyDrawer
{
    private static readonly GUIContent buttonContent = new GUIContent("Select type");

    private bool initialized = false;
    private bool isValid = true;

    private (Type type, string fullPath)[] possibleTypes;
    private string[] typeNames;

    private SerializedObject targetObject;

    private SerializedPropertyReflect reflectionInfo;

    private Type[] bannedTypes = new Type[0];

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!initialized) Initialize(property);

        if (!isValid)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        var propRect = position;
        var controlRect = position;
        controlRect.height = EditorGUIUtility.singleLineHeight;

        label.text += " ->Type: " + property.ManagedRefType();
        controlRect = EditorGUI.PrefixLabel(controlRect, label);

        if (GUI.Button(controlRect, buttonContent))
            ShowContextMenu(property);

        var diff = new Vector2(0, controlRect.height + EditorGUIUtility.standardVerticalSpacing);
        propRect.position = propRect.position + diff;
        propRect.height = position.height - diff.y;

        EditorGUI.PropertyField(propRect, property, new GUIContent(property.ManagedRefType()), true);
    }

    //
    private void Initialize(SerializedProperty prop)
    {
        initialized = true;

        if (prop.propertyType != SerializedPropertyType.ManagedReference)
        {
            isValid = false;
            return;
        }

        targetObject = prop.serializedObject;

        reflectionInfo = prop.GetFieldInformation();

        if (reflectionInfo.field == null)
        {
            isValid = false;
            return;
        }

        var bannedTypes = reflectionInfo.field.GetCustomAttribute<PolymorphEditorAttribute>().BannedTypes;

        //var value = reflectionInfo.GetValue(prop);
        //Debug.Log(reflectionInfo.containingType);

        CacheTypes(reflectionInfo.field.FieldType, bannedTypes);
    }

    private void CacheTypes(Type fieldType, IEnumerable<Type> bannedTypes)
    {
        possibleTypes = fieldType.GetInheritingTypesWithPaths(bannedTypes: bannedTypes);

        typeNames = possibleTypes.Select(t => t.type.Name).ToArray();
    }

    private void ShowContextMenu(SerializedProperty property)
    {
        var menu = new GenericMenu();

        var currentIndex = typeNames.GetIndexOf(property.ManagedRefType());

        for (var i = 0; i < possibleTypes.Length; i++)
        {
            var t = possibleTypes[i];

            var selected = i == currentIndex;

            menu.AddItem(new GUIContent(t.fullPath), selected, () =>
            {
                if (selected) return;

                SetObjectAsType(property, t.type);
            });
        }

        menu.AddItem(new GUIContent("Null"), false, () => { SetNull(property); });

        menu.ShowAsContext();
    }

    private void SetNull(SerializedProperty property)
    {
        reflectionInfo.SetValue(property, null);
        targetObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(targetObject.targetObject);
    }

    private void SetObjectAsType(SerializedProperty prop, Type type)
    {
        if (!type.TryCreateDefaultInstance(out var newObj))
        {
            Debug.LogError("Can not create instance for type " + type +
                " in polymorph list because it has no constructor with 0 parameters");
            return;
        }

        reflectionInfo.SetValue(prop, newObj);
        //prop.managedReferenceValue = newObj;

        targetObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(targetObject.targetObject);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!initialized) Initialize(property);

        return (initialized && isValid)
            ? reflectionInfo.GetValue(property) is { } ? EditorGUI.GetPropertyHeight(property, true) +
                                                         EditorGUIUtility.singleLineHeight +
                                                         EditorGUIUtility.standardVerticalSpacing :
            EditorGUIUtility.singleLineHeight
            : EditorGUI.GetPropertyHeight(property, true);
    }
}