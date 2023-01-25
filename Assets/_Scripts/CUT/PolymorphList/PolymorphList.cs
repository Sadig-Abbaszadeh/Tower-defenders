using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PolymorphList<T> : IList<T> where T : class
{
    [SerializeReference]//, SerializeField]
    public List<T> collection = new List<T>();

    public T this[int index]
    {
        get => collection[index];
        set => collection[index] = value;
    }

    public int Count => collection.Count;

    public bool IsReadOnly => false;

    public void Add(T item) => collection.Add(item);

    public void Clear() => collection.Clear();

    public bool Contains(T item) => collection.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();

    public int IndexOf(T item) => collection.IndexOf(item);

    public void Insert(int index, T item) => collection.Insert(index, item);

    public bool Remove(T item) => collection.Remove(item);

    public void RemoveAt(int index) => collection.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => collection.GetEnumerator();
}