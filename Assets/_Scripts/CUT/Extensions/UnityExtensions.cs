using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DartsGames.CUT.UnityExtensions
{
    public static class UnityExtensions
    {
        public static IEnumerator SmoothLookAt(this Transform tr, Vector3 forward, float lookTime = 2f, float maxAngleToIgnoreLook = 6f)
        {
            var initRot = tr.rotation;
            var targetRot = Quaternion.LookRotation(forward);

            if (Quaternion.Angle(initRot, targetRot) < maxAngleToIgnoreLook)
                yield break;

            var t = 0f;
            var incr = 1 / lookTime;

            while(t < 1)
            {
                t += incr * Time.deltaTime;

                tr.rotation = Quaternion.Slerp(initRot, targetRot, t);

                yield return null;
            }
        }

        /// <summary>
        /// set active for all parents too
        /// </summary>
        public static void SetActiveUpwrards(this GameObject gameObject, bool active)
        {
            gameObject.SetActive(active);

            var parent = gameObject.transform.parent;

            while(parent)
            {
                parent.gameObject.SetActive(active);
                parent = parent.parent;
            }
        }

        public static float RandomFromZero(this float f) => UnityEngine.Random.Range(0, f);

        public static void ToggleBool(this Animator animator, string paramName) => animator.SetBool(paramName, !animator.GetBool(paramName));

        public static void DoAtTheEndOfFrame(this MonoBehaviour mb, Action action) =>
            mb.StartCoroutine(EndOfFrameAction(action));

        public static void TryStop(this MonoBehaviour mb, ref Coroutine cor)
        {
            if (cor != null)
            {
                mb.StopCoroutine(cor);
                cor = null;
            }
        }

        public static Coroutine DoAfterOneFrame(this MonoBehaviour mb, Action action) =>
            mb.StartCoroutine(ActionAfterFrame(action));

        public static Coroutine DoAfterTime(this MonoBehaviour mb, float time, Action action) =>
            mb.StartCoroutine(ActionAfterTime(time, action));

        public static Coroutine DoAfterUnscaledTime(this MonoBehaviour mb, float time, Action action) =>
            mb.StartCoroutine(ActionAfterUnscaledTime(time, action));

        public static Coroutine DoWhile(this MonoBehaviour mb, Func<bool> waitCondition, Action action) =>
            mb.StartCoroutine(DoWhileTrue(waitCondition, action));

        public static Coroutine DoWhile(this MonoBehaviour mb, Func<bool> waitCondition, Action action,
            Action afterwards) => mb.StartCoroutine(DoWhileTrue(waitCondition, action, afterwards));

        public static Coroutine DoAfterConditionIsTrue(this MonoBehaviour mb, Func<bool> condition, Action action) =>
            mb.StartCoroutine(DoAfterTrue(condition, action));

        public static Coroutine DoEvery(this MonoBehaviour mb, float waitTime, Action action) =>
            mb.StartCoroutine(DoEvery(waitTime, waitTime, action));

        public static Coroutine DoEvery(this MonoBehaviour mb, float waitTime, float initialWaitTime, Action action) =>
            mb.StartCoroutine(DoEvery(waitTime, initialWaitTime, action));

        public static Coroutine DoAfterRoutine(this MonoBehaviour mb, Coroutine routine, Action action) =>
            mb.StartCoroutine(DoAfterOtherRoutine(routine, action));

        public static void DebugCollection<T>(this IEnumerable<T> colelction)
        {
            foreach (var element in colelction)
                UnityEngine.Debug.Log(element);
        }

        /// <summary>
        /// Similar to GetCompInParent but excludes this gameobject
        /// </summary>
        public static T GetCompInAncestors<T>(this Component c)
        {
            Transform t = c.transform.parent;

            while (t != null)
            {
                if (t.TryGetComponent<T>(out var comp))
                    return comp;

                t = t.parent;
            }

            return default;
        }

        /// <summary>
        /// Sets layer for this game object and all of its children in hierarchy
        /// </summary>
        public static void SetLayerAll(this GameObject go, int layer)
        {
            go.layer = layer;

            foreach (Transform t in go.transform)
            {
                t.gameObject.SetLayerAll(layer);
            }
        }

        public static Color ToTransparent(this Color color) => new Color(color.r, color.g, color.b, 0f);
        public static Color ToOpaque(this Color color) => new Color(color.r, color.g, color.b, 1f);
        public static void MakeOpaque(this Image image) => image.color = image.color.ToOpaque();
        public static void MakeTransparent(this Image image) => image.color = image.color.ToTransparent();

        public static void Destroy(this UnityEngine.Object obj) => UnityEngine.Object.Destroy(obj);
        public static void DestroyImmediate(this UnityEngine.Object obj) => UnityEngine.Object.DestroyImmediate(obj);

        public static T Debug<T>(this T t)
        {
            UnityEngine.Debug.Log(t);
            return t;
        }

        public static T Debug<T>(this T t, UnityEngine.Object context)
        {
            UnityEngine.Debug.Log(t, context);
            return t;
        }

        public static bool ContainsLayer(this LayerMask layerMask, int layer) =>
            layerMask == (layerMask | (1 << layer));

        public static void SetParent(this Transform t, Component parent) => t.SetParent(parent.transform);

        public static Transform GetRandomChild(this Transform t) =>
            t.GetChild(UnityEngine.Random.Range(0, t.childCount));

        public static Vector2 XZ(this Vector3 v) => new Vector2(v.x, v.z);

        #region Behind the scenes

        private static IEnumerator ActionAfterFrame(Action action)
        {
            yield return null;
            action();
        }

        private static IEnumerator ActionAfterTime(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action();
        }

        private static IEnumerator ActionAfterUnscaledTime(float time, Action action)
        {
            yield return new WaitForSecondsRealtime(time);
            action();
        }

        private static IEnumerator DoWhileTrue(Func<bool> condition, Action action)
        {
            while (condition())
            {
                action();
                yield return null;
            }
        }

        private static IEnumerator DoWhileTrue(Func<bool> condition, Action action, Action afterwards)
        {
            while (condition())
            {
                action();
                yield return null;
            }

            afterwards();
        }

        private static IEnumerator DoAfterTrue(Func<bool> condition, Action action)
        {
            yield return new WaitUntil(condition);
            action();
        }

        private static IEnumerator DoEvery(float time, float beginTime, Action action)
        {
            yield return new WaitForSeconds(beginTime);
            action();

            while (true)
            {
                yield return new WaitForSeconds(time);
                action();
            }
        }

        private static IEnumerator EndOfFrameAction(Action action)
        {
            yield return new WaitForEndOfFrame();
            action();
        }

        private static IEnumerator DoAfterOtherRoutine(Coroutine routine, Action action)
        {
            yield return routine;

            action();
        }

        #endregion

        #region Lerp
        public static bool Lerp(this bool a, bool b, float t) => t >= 0.5f ? b : a;
        #endregion

    }
}