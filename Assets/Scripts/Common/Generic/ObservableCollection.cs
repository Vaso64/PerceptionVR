using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace PerceptionVR.Common.Generic
{
    public class ObservableCollection<T> : ICollection<T>, INotifyCollectionChanged<T>
    {
        private readonly List<T> collection = new ();
        
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
            OnRemoved?.Invoke(new []{item});
            return collection.Remove(item);
        }
        
        public bool RemoveRange(IEnumerable<T> items)
        {
            OnRemoved?.Invoke(items);
            return collection.RemoveAll(items.Contains) > 0;
        }
        
        public void Clear()
        {
            OnRemoved?.Invoke(collection);
            collection.Clear();
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