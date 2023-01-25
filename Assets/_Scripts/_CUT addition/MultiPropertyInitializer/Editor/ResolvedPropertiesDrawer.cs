using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ResolvedPropertiesDrawer<T> : PropertyDrawer
{
    protected Dictionary<string, T> propertyBindings = new Dictionary<string, T>();

    protected virtual T InitializeProperty(SerializedProperty prop) => default;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _Init(property);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        _Init(property);

        return base.GetPropertyHeight(property, label);
    }

    private void _Init(SerializedProperty property)
    {
        if (!propertyBindings.ContainsKey(property.propertyPath))
            propertyBindings.Add(property.propertyPath, InitializeProperty(property));
    }
}