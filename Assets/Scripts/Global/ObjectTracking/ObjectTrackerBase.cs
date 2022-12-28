 using System;
using System.Collections;
using System.Collections.Generic;
 using System.Linq;
using UnityEngine;
 using PerceptionVR.Common.Generic;

namespace PerceptionVR.Global
{
    public abstract class ObjectTrackerBase<T> : INotifyCollectionChanged<T>, IEnumerable<T>
    {
        public event Action<IEnumerable<T>> OnAdded;
        public event Action<IEnumerable<T>> OnRemoved;
        public IEnumerator<T> GetEnumerator() => allObjects.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected readonly List<T> allObjects = new ();

        public ObjectTrackerBase()
        {
            // Register events
            GlobalEvents.OnAfterInstantiate += OnAfterInstantiateCallback;
            GlobalEvents.OnBeforeDestroy += OnBeforeDestroyCallback;
            GlobalEvents.OnAfterAddComponent += OnAfterAddComponentCallback;
            GlobalEvents.OnBeforeRemoveComponent += OnBeforeRemoveComponentCallback;
        }

        private void OnAfterInstantiateCallback(GameObject gameObject)
        {
            var components = gameObject.GetComponentsInChildren<T>();
            allObjects.AddRange(components);
            OnAdded?.Invoke(components);
        }

        private void OnBeforeDestroyCallback(GameObject gameObject)
        {
            var components = gameObject.GetComponentsInChildren<T>();
            allObjects.RemoveAll(c => components.Contains(c));
            OnRemoved?.Invoke(components);
        }

        private void OnAfterAddComponentCallback(Component component)
        {
            if (component is not T t) 
                return;
            allObjects.Add(t);
            OnAdded?.Invoke(new[] {t});
        }

        private void OnBeforeRemoveComponentCallback(Component component)
        {
            if (component is not T t) 
                return;
            allObjects.Remove(t);
            OnRemoved?.Invoke(new[] {t});
        }
    }
}