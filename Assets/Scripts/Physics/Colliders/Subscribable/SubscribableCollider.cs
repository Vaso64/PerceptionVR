using System;
using System.Collections.Generic;
using UnityEngine;

namespace PerceptionVR.Physics
{
    public class SubscribableCollider : ColliderBase
    {
        private Queue<Collision> onCollisionEnterQueue = new();
        public event Action<Collision> onCollisionEnter;
    
        private Queue<Collision> onCollisionExitQueue = new();
        public event Action<Collision> onCollisionExit;

        private void FixedUpdate()
        {
            while (onCollisionEnterQueue.Count > 0)
                onCollisionEnter?.Invoke(onCollisionEnterQueue.Dequeue());
        
            while (onCollisionExitQueue.Count > 0)
                onCollisionExit?.Invoke(onCollisionExitQueue.Dequeue());
        }

        private void OnCollisionEnter(Collision collision) => onCollisionEnterQueue.Enqueue(collision);

        private void OnCollisionExit(Collision collision) => onCollisionExitQueue.Enqueue(collision);
    }
}