using System;
using System.Collections.Generic;
using UnityEngine;

namespace DartsGames.CUT
{
    public class DontDestroy : MonoBehaviour
    {
        private static List<GameObject> objects = new List<GameObject>();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            objects.Add(gameObject);
        }

        public static void DestroyAll()
        {
            for(int i = 0; i < objects.Count; i++)
            {
                Destroy(objects[i]);
            }

            objects.Clear();
        }
    }
}