 using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using PerceptionVR.Common.Generic;

namespace PerceptionVR.Global
{
    public class ComponentTracker<T> : INotifyCollectionChanged<T>, IEnumerable<T> where T : Component
    {
        public event Action<IEnumerable<T>> OnAdded;
        public event Action<IEnumerable<T>> OnRemoved;
        public IEnumerator<T> GetEnumerator() => allComponents.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private readonly List<T> allComponents = new ();

        public ComponentTracker()
        {
            // Register events
            GlobalEvents.OnAfterInstantiate += OnAfterInstantiateCallback;
            GlobalEvents.OnBeforeDestroy += OnBeforeDestroyCallback;
            GlobalEvents.OnAfterAddComponent += OnAfterAddComponentCallback;
            GlobalEvents.OnBeforeRemoveComponent += OnBeforeRemoveComponentCallback;
            
            // Store all components T in scene
            var components = Object.FindObjectsOfType<T>();
            allComponents.AddRange(components);
        }

        private void OnAfterInstantiateCallback(GameObject gameObject)
        {
            var components = gameObject.GetComponentsInChildren<T>();
            allComponents.AddRange(components);
            OnAdded?.Invoke(components);
        }

        private void OnBeforeDestroyCallback(GameObject gameObject)
        {
            var components = gameObject.GetComponentsInChildren<T>();
            allComponents.RemoveAll(c => components.Contains(c));
            OnRemoved?.Invoke(components);
        }

        private void OnAfterAddComponentCallback(Component component)
        {
            if (component is not T t) 
                return;
            allComponents.Add(t);
            OnAdded?.Invoke(new[] {t});
        }

        private void OnBeforeRemoveComponentCallback(Component component)
        {
            if (component is not T t) 
                return;
            allComponents.Remove(t);
            OnRemoved?.Invoke(new[] {t});
        }
    }
}