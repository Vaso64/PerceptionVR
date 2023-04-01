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
        [SerializeField] protected List<T> collection = new();
        
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
            if(res)
                OnRemoved?.Invoke(new []{item});
            return res;
        }
        
        public void RemoveRange(IEnumerable<T> items)
        {
            var toRemove = collection.Intersect(items);
            collection.RemoveAll(toRemove.Contains);
            OnRemoved?.Invoke(toRemove);
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
        public virtual IEnumerator<T> GetEnumerator() => collection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => collection.GetEnumerator();
        public bool IsReadOnly => false;
    }
}