using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DartsGames.CUT.Utils
{
    public static class Utilities
    {
        #region UI Raycast        
        public static List<RaycastResult> UiRaycast<T>(Vector2 screenPosition, T hasComponent) => (List<RaycastResult>)UiRaycast(screenPosition).Where((RaycastResult element) => element.gameObject.TryGetComponent(out T t));

        public static List<RaycastResult> UiRaycast<T>(PointerEventData eventData, T hasComponent) => (List<RaycastResult>)UiRaycast(eventData).Where((RaycastResult element) => element.gameObject.TryGetComponent(out T t));

        public static List<RaycastResult> UiRaycast(Vector2 screenPosition)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = screenPosition;
            return UiRaycast(eventData);
        }

        public static List<RaycastResult> UiRaycast(PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results;
        }
        #endregion

        #region IO
        /// <summary>
        /// Find directories relative to path and its sub directories that match the search pattern. Search pattern can be the name of the folder being searched
        /// </summary>
        public static string[] GetAllDirectoryPaths(string path, string searchPattern) => Directory.GetDirectories(path, searchPattern, SearchOption.AllDirectories);

        public static bool FindFilePath(string fileName, string folderPath, out string filePath)
        {
            filePath = "";
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath);

                foreach (var f in files)
                {
                    if (f.Equals(Path.Combine(folderPath, fileName)))
                    {
                        filePath = f;
                        return true;
                    }
                }

                var folders = Directory.GetDirectories(folderPath);

                foreach (var f in folders)
                    if (FindFilePath(fileName, f, out filePath))
                        return true;

            }

            return false;
        }
        #endregion
    }

    public static class ReflectionUtils
    {
        public static BindingFlags GetStandardFlags() => BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
    }

    public class MapRange
    {
        private float a, c, k;

        //
        public MapRange(float rangeBegin, float rangeEnd, float newRangeBegin, float newRangeEnd)
        {
            if (rangeBegin == rangeEnd)
                throw new ArithmeticException("Can not map a constant to a range");

            a = rangeBegin;
            c = newRangeBegin;
            k = (newRangeEnd - newRangeBegin) / (rangeEnd - rangeBegin);
        }

        public float Map(float value) => (value - a) * k + c;

        public static float MapDynamic(float value, float rangeBegin, float rangeEnd, float newRangeBegin, float newRangeEnd)
            => (value - rangeBegin) * (newRangeEnd - newRangeBegin) / (rangeEnd - rangeBegin) + newRangeBegin;
    }
}