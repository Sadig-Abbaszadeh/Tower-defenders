using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DartsGames.CUT
{
    [DisallowMultipleComponent]
    public class PoolObject : MonoBehaviour
    {
        [HideInInspector] public int objectType;

        private List<IPoolComponent> behaviours;

        public event Action OnStart;
        public event Action OnDestroy;

        private void Awake()
        {
            behaviours = GetComponents<IPoolComponent>().ToList();
            behaviours.ForEach(b => b.poolObject = this);

            OnStart += () =>
            {
                gameObject.SetActive(true);
            };
        }

        public T GetPoolObjectComponent<T>() where T : IPoolComponent
        {
            foreach (var b in behaviours)
            {
                if (b is T t)
                {
                    return t;
                }
            }

            return default(T);
        }

        public void Destroy()
        {
            OnDestroy();
            gameObject.SetActive(false);
        }

        #region BuilderPattern
        public PoolObject StartBehaviours()
        {
            OnStart();
            return this;
        }

        public PoolObject SetParent(Transform parent)
        {
            transform.SetParent(parent);
            return this;
        }

        public PoolObject SetWorldPositionAndLocation(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
            return this;
        }

        public PoolObject SetLocalPositionAndLocation(Vector3 position, Quaternion rotation)
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
            return this;
        }
        #endregion
    }
}