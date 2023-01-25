using UnityEngine;
using System;

namespace DartsGames.CUT.Attributes
{ 
    public class ReadonlyAttribute : PropertyAttribute { }

    /// <summary>
    /// Add this attribute to the monobehaviour classes to be able to use Extended Editor features
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtendEditorAttribute : Attribute
    { }

    /// <summary>
    /// Only works on methods with no parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class InspectorButtonAttribute : Attribute
    {
    }

    /// <summary>
    /// Implement this attribute to quickly pack fields into horizontal area
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class HorizontalAreaAttribute : Attribute
    {
        /// <summary>
        /// Name of the horizontal group
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Order inside area (ascending)
        /// </summary>
        public int order = 0;

        public HorizontalAreaAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Use this to serialize a field with condition. MemberName is method or field name, and value is the value which will be compared to field/method return value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class InspectConditionAttribute : PropertyAttribute
    {
        public string MemberName { get; private set; } = "";
        public object Value { get; private set; } = default;

        public bool Reverse { get; private set; }

        public InspectConditionAttribute(string memberName, object value, bool reverse = false)
        {
            MemberName = memberName;
            Value = value;
            Reverse = reverse;
        }
    }

    /// <summary>
    /// Works for UnityEngine.Object-s only
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InlineEditorAttribute : PropertyAttribute
    {
    }

    /// <summary>
    /// Works for UnityEngine.Object-s only
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : PropertyAttribute
    {
        public string message { get; private set; }
        public RequiredMessageType messageType { get; private set; }

        public RequiredAttribute(string message = null, RequiredMessageType messageType = RequiredMessageType.Info)
        {
            this.message = message;
            this.messageType = messageType;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TabGroupAttribute : PropertyAttribute
    {
        public string tabName { get; private set; } = null;

        public TabGroupAttribute(string tabName)
        {
            this.tabName = tabName;
        }
    }
}