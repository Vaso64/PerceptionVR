using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerceptionVR.Common.Generic
{
    public abstract class MonoBehaviourCollection<T> : MonoBehaviourBase, ICollection<T>
    {
        [SerializeField] private List<T> collection = new();
        
        // ICollection implementation
        public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void Add(T item) => collection.Add(item);
        public void Clear() => collection.Clear();
        public bool Contains(T item) => collection.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);
        public bool Remove(T item) => collection.Remove(item);
        public int Count => collection.Count;
        public bool IsReadOnly => false;
    }
}