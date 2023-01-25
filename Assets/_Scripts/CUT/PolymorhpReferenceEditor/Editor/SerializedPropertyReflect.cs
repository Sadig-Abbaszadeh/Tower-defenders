using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SerializedPropertyReflect
{
    public FieldInfo field;
    public object containingObject;

    public ContainingType containingType;

    public PropertyInfo itemP;

    public Type PropertyType
    {
        get
        {
            switch (containingType)
            {
                case ContainingType.Array:
                case ContainingType.List:
                    return field.FieldType.GenericTypeArguments[0];
                case ContainingType.Object:
                    return field.FieldType;
                default:
                    return null;
            }
        }
    }

    public object GetValue(SerializedProperty serP)
    {
        switch (containingType)
        {
            case ContainingType.Array:
                return (containingObject as Array).GetValue(GetIndexOfProperty(serP));
            case ContainingType.List:
                return itemP.GetValue(containingObject, new object[] { GetIndexOfProperty(serP) });
            case ContainingType.Object:
                return field.GetValue(containingObject);
            default:
                throw new Exception("unknown exception");
        }
    }

    public void SetValue(SerializedProperty serP, object value)
    {
        switch (containingType)
        {
            case ContainingType.Array:
                (containingObject as Array).SetValue(value, GetIndexOfProperty(serP));
                break;
            case ContainingType.List:
                itemP.SetValue(containingObject, value, new object[] { GetIndexOfProperty(serP) });
                break;
            case ContainingType.Object:
                if (serP.propertyType == SerializedPropertyType.ManagedReference)
                    serP.managedReferenceValue = value;
                else
                      field.SetValue(containingObject, value);
                break;
            default:
                break;
        }
    }

    private int GetIndexOfProperty(SerializedProperty serP)
    {
        var fullPath = serP.propertyPath;

        var l = fullPath.LastIndexOf('[');
        var r = fullPath.LastIndexOf(']');

        var sub = fullPath.Substring(l + 1, r - l - 1);

        if (!int.TryParse(sub, out var index))
            throw new System.Exception("Property path does not contain list index");

        return index;
    }

    public enum ContainingType
    {
        Array,
        List,
        Object,
    }
}