using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using DartsGames.CUT.CoreExtensions;
using DartsGames.CUT.UnityExtensions;
using System.Collections.Generic;

namespace DartsGames.CUT.EditorExtensions
{
    public static class EditorExtensions
    {
        public static SerializedPropertyReflect GetFieldInformation(this SerializedProperty serP)
        {
            var target = serP.serializedObject.targetObject;
            var fullPath = serP.propertyPath.Split('.');

            //fullPath.DebugCollection();

            var reflect = new SerializedPropertyReflect();
            reflect.containingObject = target;

            for (int i = 0; i < fullPath.Length; i++)
            {
                reflect.containingType = SerializedPropertyReflect.ContainingType.Object;

                var p = fullPath[i];

                reflect.field = reflect.containingObject.GetType().
                    GetField(p, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // is further nested
                if(i < fullPath.Length - 1)
                {
                    string dataString;

                    reflect.containingObject = reflect.field.GetValue(reflect.containingObject);

                    // is array or list
                    if (fullPath[i + 1].Contains("Array") && i < fullPath.Length - 2 && 
                        (dataString = fullPath[i + 2]).Contains("["))
                    {
                        var l = dataString.IndexOf('[');
                        var r = dataString.IndexOf(']');

                        var indexStr = dataString.Substring(l + 1, r - l - 1);

                        if (!int.TryParse(indexStr, out var index))
                            throw new Exception("Unknown index exception");

                        // determine if is array or list
                        if(reflect.containingObject is Array arr)
                        {
                            reflect.containingObject = arr.GetValue(index);
                            reflect.containingType = SerializedPropertyReflect.ContainingType.Array;
                        }
                        else if(reflect.field.FieldType.IsList())
                        {
                            //Debug.Log("list");
                            reflect.itemP = reflect.field.FieldType.GetProperty("Item");

                            reflect.containingObject = reflect.itemP.GetValue(reflect.containingObject, new object[] { index });
                            reflect.containingType = SerializedPropertyReflect.ContainingType.List;
                        }
                        else
                        {
                            throw new NotSupportedException("This type is not supported u dumb");
                        }

                        i += 2;
                    }
                }
            }

            return reflect;
        }

        public static string ManagedRefType(this SerializedProperty serP)
        {
            var fullType = serP.type;

            if (!fullType.Contains("managedReference"))
                return string.Empty;

            return fullType.Substring(17, fullType.Length - 17 - 1);
        }

        public static void LookTowardsPoint(this SceneView sceneView, Vector3 point) =>
            LookTowardsDirection(sceneView, point - sceneView.camera.transform.position);

        public static void LookTowardsDirection(this SceneView sceneView, Vector3 direction) =>
            sceneView.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}