using UnityEngine;

namespace DartsGames.CUT
{
    /// <summary>
    /// A wrapper class to serialize interfaces. 
    /// </summary>
    [System.Serializable]
    public class SerializedType<T> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private Component c;

        [System.NonSerialized]
        public T value = default;

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            if (value is null && c is T t)
                value = t;
        }
    }
}