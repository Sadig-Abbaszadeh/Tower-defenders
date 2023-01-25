using System;
using System.Collections.Generic;
using System.Linq;

namespace DartsGames.Extensions
{
    public static class TypeExtensions
    {
        public static string[] GetTracebackTo(this Type type, Type to)
        {
            //UnityEngine.Debug.Log(type + " ; " + to);

            var result = new List<string>();
            var currentType = type;

            while (currentType.BaseType != null && currentType.BaseType != to && currentType.BaseType != typeof(object))
            {
                result.Add(currentType.BaseType.Name);
                currentType = currentType.BaseType;
            }

            return result.ToArray();
        }

        public static bool TryCreateDefaultInstance(this Type t, out object instance)
        {
            if (t.GetConstructors().Any(c => c.GetParameters().Length == 0))
            {
                instance = Activator.CreateInstance(t);
                return true;
            }

            instance = null;
            return false;
        }
    }
}
