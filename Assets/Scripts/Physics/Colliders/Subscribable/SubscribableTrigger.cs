using System;
using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Debug;
using UnityEngine;

namespace PerceptionVR.Physics
{
    public class SubscribableTrigger : ColliderBase, IEnumerable<Collider>
    {
        public IEnumerator<Collider> GetEnumerator() => collidersInside.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        [SerializeField] protected List<Collider> collidersInside = new();
        public List<Collider> collidersInsideLastFrame = new();
    
        protected readonly Queue<Collider> onTriggerEnterQueue = new();
        public event Action<Collider> onTriggerEnter;
        
        protected readonly Queue<Collider> onTriggerExitQueue = new();
        public event Action<Collider> onTriggerExit;
        
        private bool isProcessingQueue;
        protected event Action OnBeforeProcessQueue;
    
        protected override void Awake()
        {
            base.Awake();
            collider.isTrigger = true;
            StartCoroutine(ProcessQueueRoutine());
        }
        
        protected void ProcessQueue()
        {
            if (!isProcessingQueue)
                StartCoroutine(ProcessQueueRoutine());
        }
        
        private IEnumerator ProcessQueueRoutine()
        {
            isProcessingQueue = true;
            yield return new WaitForFixedUpdate();
            OnBeforeProcessQueue?.Invoke();
            
            // Trigger enter
            while (onTriggerEnterQueue.Count > 0)
            {
                var other = onTriggerEnterQueue.Dequeue();
                //Debugger.LogInfo($"OnTriggerEnter {other} to {this}");
                onTriggerEnter?.Invoke(other);
            }
            
            // Trigger exit
            while (onTriggerExitQueue.Count > 0)
            {
                var other = onTriggerExitQueue.Dequeue();
                //Debugger.LogInfo($"OnTriggerExit {other} from {this}");
                onTriggerExit?.Invoke(other);
            }

            // Update last frame
            collidersInsideLastFrame.Clear();
            collidersInsideLastFrame.AddRange(collidersInside);
            
            isProcessingQueue = false;
        }
    
        protected virtual void OnTriggerEnter(Collider other)
        {
            collidersInside.Add(other);
            onTriggerEnterQueue.Enqueue(other);
            ProcessQueue();
        }
    
        protected virtual void OnTriggerExit(Collider other)
        {
            collidersInside.Remove(other);
            onTriggerExitQueue.Enqueue(other);
            ProcessQueue();
        }
    }
}
