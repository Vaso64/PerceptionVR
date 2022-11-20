using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PerceptionVR.Common.Generic
{
    [System.Serializable]
    public class ObservableCollection<T> : ICollection<T>, INotifyCollectionChanged<T>
    {
        [SerializeField] private List<T> collection = new();
        
        public event Action<IEnumerable<T>> OnAdded;
        public event Action<IEnumerable<T>> OnRemoved;
        
        public void Add(T item)
        {
            collection.Add(item);
            OnAdded?.Invoke(new []{item});
        }
        
        public void AddRange(IEnumerable<T> items)
        {
            collection.AddRange(items);
            OnAdded?.Invoke(items);
        }
        
        public bool Remove(T item)
        {
            var res = collection.Remove(item);
            OnRemoved?.Invoke(new []{item});
            return res;
        }
        
        public void RemoveRange(IEnumerable<T> items)
        {
            var res = collection.RemoveAll(items.Contains) > 0;
            OnRemoved?.Invoke(items);
        }
        
        public void Clear()
        {
            var items = collection.ToList();
            collection.Clear();
            OnRemoved?.Invoke(items);
        }

        // Rest of the implementation (default)
        public bool Contains(T item) => collection.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);
        public int Count => collection.Count;
        public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => collection.GetEnumerator();
        public bool IsReadOnly => false;
    }
}