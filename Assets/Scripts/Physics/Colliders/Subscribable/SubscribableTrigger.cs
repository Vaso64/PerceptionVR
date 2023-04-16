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
        [SerializeField] public List<Collider> collidersInsideLastFrame = new();
    
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
            while (onTriggerEnterQueue.Count > 0)
            {
                var other = onTriggerEnterQueue.Dequeue();
                //Debugger.LogInfo($"OnTriggerEnter {other} to {this}");
                onTriggerEnter?.Invoke(other);
            }
            
            while (onTriggerExitQueue.Count > 0)
            {
                var other = onTriggerExitQueue.Dequeue();
                //Debugger.LogInfo($"OnTriggerExit {other} from {this}");
                onTriggerExit?.Invoke(other);
            }
            isProcessingQueue = false;
        }
    
        protected virtual void OnTriggerEnter(Collider other)
        {
            collidersInside.Add(other);
            StartCoroutine(AddLastFrame(other));
            onTriggerEnterQueue.Enqueue(other);
            ProcessQueue();
        }
    
        protected virtual void OnTriggerExit(Collider other)
        {
            collidersInside.Remove(other);
            StartCoroutine(RemoveLastFrame(other));
            onTriggerExitQueue.Enqueue(other);
            ProcessQueue();
        }
        
        protected IEnumerator AddLastFrame(Collider other)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            collidersInsideLastFrame.Add(other);
        }

        protected IEnumerator RemoveLastFrame(Collider other)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            collidersInsideLastFrame.Remove(other);
        }
    }
}
