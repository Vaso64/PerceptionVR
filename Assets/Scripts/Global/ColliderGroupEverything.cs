using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PerceptionVR.Global
{
    public class ColliderGroupEverything : ColliderGroup
    {
        public ColliderGroupEverything()
        {
            // Store all colliders in scene
            MonoBehaviourEvents.OnStart += () =>
            {
                foreach (var collider in Object.FindObjectsOfType<Collider>())
                    Add(collider);
            };
            
            // Register events
            GlobalEvents.OnAfterInstantiate += OnAfterInstantiateCallback;
            GlobalEvents.OnBeforeDestroy += OnBeforeDestroyCallback;
            GlobalEvents.OnAfterAddComponent += OnAfterAddComponentCallback;
            GlobalEvents.OnBeforeRemoveComponent += OnBeforeRemoveComponentCallback;
        }
        
        private void OnAfterInstantiateCallback(GameObject gameObject)
        {
            // Add all colliders in the gameObject
            foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
                Add(collider);
        }

        private void OnBeforeDestroyCallback(GameObject gameObject)
        {
            // Remove all colliders in the gameObject
            foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
                Remove(collider);
        }

        private void OnAfterAddComponentCallback(Component component)
        {
            // Add collider if it is a collider
            if (component is Collider collider)
                Add(collider);
        }

        private void OnBeforeRemoveComponentCallback(Component component)
        {
            // Remove collider if it is a collider
            if (component is Collider collider)
                Remove(collider);
        }
    }
}