using UnityEditor;
using UnityEngine;

namespace DartsGames.CUT.Editors
{
    [CustomEditor(typeof(CreateObjectsAroundThis))]
    public class CircleObjectCreatorEditor : Editor
    {
        private CreateObjectsAroundThis t;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            t = target as CreateObjectsAroundThis;
            Transform parent = t.transform;

            if (GUILayout.Button("Generate"))
            {
                t.Generate();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear list"))
            {
                t.objects = new Transform[0];
            }
            if (GUILayout.Button("Clear all"))
            {
                t.DestroyAll();
                t.objects = new Transform[0];
            }
            GUILayout.EndHorizontal();
        }
    }
}