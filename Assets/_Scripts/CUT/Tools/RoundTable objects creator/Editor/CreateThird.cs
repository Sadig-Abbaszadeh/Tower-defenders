using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DartsGames.CUT.Editors
{
    public static class CreateThird
    {
        [MenuItem("DartsGames/CUT/CreateThirdObject/Upwards")]
        private static void CreateThirdUpwards() => CreateThirdObject(true);
        [MenuItem("DartsGames/CUT/CreateThirdObject/Downwards")]
        private static void CreateThirdDownwards() => CreateThirdObject(false);

        [MenuItem("DartsGames/CUT/CreateThirdObject/Upwards", validate = true), MenuItem("DartsGames/CreateThirdObject/Downwards", validate = true)]
        private static bool CreateValidate()
        {
            return Selection.GetFiltered<Transform>(SelectionMode.ExcludePrefab).Length == 2;
        }

        private static void CreateThirdObject(bool upwards)
        {
            var sel = Selection.GetFiltered<Transform>(SelectionMode.ExcludePrefab);

            Selection.activeGameObject = null;
            Selection.activeGameObject = sel[0].gameObject;

            Unsupported.CopyGameObjectsToPasteboard();
            Unsupported.PasteGameObjectsFromPasteboard();

            Vector3 position, distance;

            if (sel[0].position.y > sel[1].position.y)
                SetVectors(sel[0], sel[1], upwards, out position, out distance);
            else
                SetVectors(sel[1], sel[0], upwards, out position, out distance);

            Selection.activeGameObject.transform.position = position + distance;
        }

        private static void SetVectors(Transform one, Transform two, bool upwards, out Vector3 position, out Vector3 direction)
        {
            var sign = upwards ? 1 : -1;

            position = one.position * ((sign + 1) * .5f) + two.position * ((sign - 1) * -.5f);
            direction = (one.position - two.position) * sign;
        }
    }
}