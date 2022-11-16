using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using PerceptionVR.Global;

namespace PerceptionVR.Common.Generic
{
    // Collection of type T which last for only N frameS
    public class TemporaryCollection<T> : ICollection<T>
    {
        private readonly Dictionary<T, int> _collection = new();
        
        private readonly int frameCount;

        public TemporaryCollection(FrameType frameType, int frameCount = 0)
        {
            this.frameCount = frameCount;
            switch (frameType)
            {
                case FrameType.Fixed:
                    LateMonoBehaviourEvents.OnFixedUpdate += FrameUpdate;
                    break;
                case FrameType.Dynamic: 
                    LateMonoBehaviourEvents.OnUpdate += FrameUpdate;
                    break;
            }
        }
        
        private void FrameUpdate()
        {
            foreach (var key in _collection.Keys.ToList())
            {
                _collection[key]--;
                if (_collection[key] < 0)
                    _collection.Remove(key);
            }
        }

        // ICollection Implementation
        public IEnumerator<T> GetEnumerator() => _collection.Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _collection.Keys.GetEnumerator();
        public void Add(T item) => _collection.Add(item, frameCount);
        public void Clear() => _collection.Clear();
        public bool Contains(T item) => _collection.ContainsKey(item);
        public void CopyTo(T[] array, int arrayIndex) => _collection.Keys.CopyTo(array, arrayIndex);
        public bool Remove(T item) => _collection.Remove(item);
        public int Count => _collection.Count;
        public bool IsReadOnly => false;
    }
}