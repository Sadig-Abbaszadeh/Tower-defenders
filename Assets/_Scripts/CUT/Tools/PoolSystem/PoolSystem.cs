using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DartsGames.CUT
{
    public class PoolSystem<TKey> : MonoBehaviour
    {
        [SerializeField]
        private List<PoolingParams> poolingParams = new List<PoolingParams>();

        private static PoolSystem<TKey> Instance;

        private Dictionary<int, Queue<PoolObject>> pool = new Dictionary<int, Queue<PoolObject>>();

        [HideInInspector]
        public TKey type;

        private void Awake()
        {
            if (Instance != null)
            {
#if UNITY_EDITOR
                // edit mode
                if (!Application.isPlaying)
                    DestroyImmediate(gameObject);
                // play mode
                else
#endif
                    Destroy(gameObject);
                return;
            }

            Instance = this;

            // initialize pool
            foreach (var key in Enum.GetValues(typeof(TKey)))
                pool.Add((int)key, new Queue<PoolObject>());

            foreach (var p in poolingParams)
            {
                for (int i = 0; i < p.initialCount; i++)
                {
                    var obj = Instantiate(p.obj.gameObject).GetComponent<PoolObject>();
                    TeachObject(obj, p.objType);
                    obj.gameObject.SetActive(false);

                    AddToQueue(obj, obj.objectType);
                }
            }
        }

        // Teach poolobject how to return back to the correct pool :)
        public static void TeachObject(PoolObject obj, int type)
        {
            obj.objectType = type;
            obj.OnDestroy += () => AddToQueue(obj, type);
        }

        public static void AddToQueue(PoolObject obj, int type) => Instance.pool[type].Enqueue(obj);

        #region Instantiate
        private static PoolObject InstantiateRaw(int type)
        {
            if (Instance.pool[type].Count > 0)
                return Instance.pool[type].Dequeue();
            else
            {
                var obj = Instantiate(Instance.poolingParams.First((PoolingParams p) => p.objType == type).obj.gameObject).GetComponent<PoolObject>();
                TeachObject(obj, type);
                return obj;
            }
        }

        public static PoolObject Instantiate(int type) => InstantiateRaw(type).StartBehaviours();

        public static PoolObject Instantiate(int type, Transform parent) => InstantiateRaw(type).SetParent(parent).StartBehaviours();

        public static PoolObject Instantiate(int type, Vector3 position, Quaternion rotation) => InstantiateRaw(type).SetWorldPositionAndLocation(position, rotation).StartBehaviours();

        public static PoolObject Instantiate(int type, Vector3 localPosition, Quaternion localRotation, Transform parent = null) =>
            InstantiateRaw(type).SetParent(parent).SetLocalPositionAndLocation(localPosition, localRotation).StartBehaviours();
        #endregion
    }
}