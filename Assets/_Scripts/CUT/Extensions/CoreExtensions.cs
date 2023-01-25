using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using DartsGames.Extensions;

namespace DartsGames.CUT.CoreExtensions
{
    public static class CoreExtensions
    {
        public static int RandomIndex<T>(this IList<T> coll) => UnityEngine.Random.Range(0, coll.Count);
        public static T RandomElement<T>(this IList<T> coll) => coll[coll.RandomIndex()];
        public static T LastOf<T>(this IList<T> coll) => coll[coll.Count - 1];

        public static void ShuffleInPlace<T>(this IList<T> coll)
        {
            var n = coll.Count;
            var max = n;

            while(n > 0)
            {
                n--;
                var k = UnityEngine.Random.Range(0, max);

                var temp = coll[n];
                coll[n] = coll[k];
                coll[k] = temp;

            }
        }

        public static T RandomFirstOrDefault<T>(this IEnumerable<T> coll, Func<T, bool> predicate)
        {
            var arrTemp = coll.ToArray();
            arrTemp.ShuffleInPlace();
            return arrTemp.FirstOrDefault(predicate);
        }

        public static bool IndexInRange<T>(this IList<T> coll, int index) => index >= 0 && index < coll.Count;

        public static List<int> Indices(this IList list) =>
            new List<int>(Enumerable.Range(0, list.Count));

        public static bool ContainsAny(this string s, IEnumerable<string> checkStrings)
        {
            foreach (var check in checkStrings)
                if (s.Contains(check))
                    return true;

            return false;
        }

        public static IEnumerable<TResult> SelectIfTrue<TSource, TResult>(this IEnumerable<TSource> coll, Func<TSource, (bool select, TResult result)> selectFunction)
        {
            var result = System.Linq.Enumerable.Empty<TResult>();

            foreach (var item in coll)
            {
                var selected = selectFunction(item);

                if (selected.select)
                {
                    result = result.Append(selected.result);
                }
            }

            return result;
        }

        public static void RemoveLast<T>(this IList<T> coll) => coll.RemoveAt(coll.Count - 1);

        public static bool IsNullOrEmpty<T>(this IList<T> coll) => coll == null || coll.Count == 0;

        public static bool InRange<T>(this IList<T> coll, int index) => index >= 0 && index < coll.Count;

        public static T[] Add<T>(this T[] arr, T element)
        {
            var newArr = new T[arr.Length + 1];

            for (int i = 0; i < arr.Length; i++)
                newArr[i] = arr[i];

            newArr[arr.Length] = element;
            return newArr;
        }

        public static T FromEnd<T>(this IList<T> coll, int i) => coll[coll.Count - 1 - i];

        public static int GetIndexOf<T>(this T[] arr, T element)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Equals(element))
                    return i;
            }

            return -1;
        }

        public static List<TVal> ValuesList<TKey, TVal>(this Dictionary<TKey, TVal> dict)
        {
            var result = new List<TVal>(dict.Count);

            foreach (var pair in dict)
                result.Add(pair.Value);

            return result;
        }

        #region Reflection
        /// <summary>
        /// Useful to get private members in anchestors up the hierarcy because they are invisible to normal reflection
        /// The default binding flags are found in DartsGames.Utils.ReflectionUtils.GetStandardFlags
        /// </summary>
        public static List<MethodInfo> GetAllMethodsInAncestors(this Type t, List<Type> stopCheckTypes = null)
        {
            var methods = new List<MethodInfo>();
            var types = new List<Type>();

            while (t != null && (stopCheckTypes == default || !stopCheckTypes.Contains(t)))
            {
                types.Add(t);
                t = t.BaseType;
            }

            for (int i = types.Count - 1; i >= 0; i--)
            {
                var range = types[i].GetMethods(Utils.ReflectionUtils.GetStandardFlags());

                foreach (var m in range)
                {
                    var added = false;

                    for (int x = 0; x < methods.Count; x++)
                    {
                        if (methods[x].Name == m.Name)
                        {
                            methods[x] = m;
                            added = true;
                            break;
                        }
                    }

                    if (!added)
                        methods.Add(m);
                }
            }

            return methods;
        }

        public static bool IsList(this Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);

        public static List<FieldInfo> GetAllFieldsInAncestors(this Type t, List<Type> stopCheckTypes = null)
        {
            var fields = new List<FieldInfo>();
            var types = new List<Type>();


            while (t != null && (stopCheckTypes == default || !stopCheckTypes.Contains(t)))
            {
                types.Add(t);
                t = t.BaseType;
            }

            for (int i = types.Count - 1; i >= 0; i--)
            {
                var range = types[i].GetFields(Utils.ReflectionUtils.GetStandardFlags());

                foreach (var f in range)
                {
                    var added = false;

                    for (int x = 0; x < fields.Count; x++)
                    {
                        if (fields[x].Name == f.Name)
                        {
                            fields[x] = f;
                            added = true;
                            break;
                        }
                    }

                    if (!added)
                        fields.Add(f);
                }
            }

            return fields;
        }

        public static FieldInfo GetFieldInAncestors(this Type t, string name, List<Type> stopCheckTypes = null)
        {
            FieldInfo field = null;

            while (t != null && (stopCheckTypes == default || !stopCheckTypes.Contains(t)))
            {
                field = t.GetField(name, Utils.ReflectionUtils.GetStandardFlags());

                if (field != null)
                    break;

                t = t.BaseType;
            }

            return field;
        }

        public static MethodInfo GetMethodInAncestors(this Type t, string name, List<Type> stopCheckTypes = null)
        {
            MethodInfo method = null;

            while (t != null && (stopCheckTypes == default || !stopCheckTypes.Contains(t)))
            {
                method = t.GetMethod(name, Utils.ReflectionUtils.GetStandardFlags());

                if (method != null)
                    break;

                t = t.BaseType;
            }

            return method;
        }

        public static IEnumerable<Type> GetInheritingTypes(this Type type, bool includeAbstract = false) =>
            type.Assembly.GetTypes().
            Where(t => type.IsAssignableFrom(t) && (includeAbstract || !t.IsAbstract));

        public static (Type, string)[] GetInheritingTypesWithPaths(this Type type, bool includeAbstract = false, char separator = '/',
            IEnumerable<Type> bannedTypes = null)
        {
            bannedTypes ??= new Type[0];

            var possibleTypes = type.Assembly.GetTypes().
                Where(t => type.IsAssignableFrom(t) && (includeAbstract || !t.IsAbstract)
                && !bannedTypes.Any(banned => banned.IsAssignableFrom(t))).ToArray();

            var result = new (Type type, string fullPath)[possibleTypes.Length];

            for (int i = 0; i < possibleTypes.Length; i++)
            {
                var t = possibleTypes[i];

                var traceback = t.GetTracebackTo(type);
                var path = string.Empty;

                for (var p = traceback.Length - 1; p >= 0; p--)
                    path += traceback[p] + separator;

                result[i] = (t, path + t.Name);
            }

            // fix name issue for unity's stupid context menu
            for (int i = 0; i < result.Length; i++)
            {
                var pathToUpdate = result[i].fullPath;

                for (int j = 0; j < result.Length; j++)
                {
                    if (i == j) continue;

                    if (result[j].fullPath.Contains($"{pathToUpdate}/"))
                    {
                        result[i].fullPath += separator + "self";
                        break;
                    }
                }
            }

            return result;
        }
        #endregion
    }
}