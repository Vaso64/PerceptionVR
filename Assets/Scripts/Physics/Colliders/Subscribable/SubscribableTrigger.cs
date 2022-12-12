using System;
using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Common;
using PerceptionVR.Debug;
using PerceptionVR.Global;
using Unity.Collections;
using UnityEngine;

namespace PerceptionVR.Physics
{
    public class SubscribableTrigger : ColliderBase, IEnumerable<Collider>
    {
        public IEnumerator<Collider> GetEnumerator() => collidersInside.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        [SerializeField] protected List<Collider> collidersInside = new();
    
        protected readonly Queue<Collider> onTriggerEnterQueue = new();
        public event Action<Collider> onTriggerEnter;
        
        protected readonly Queue<Collider> onTriggerExitQueue = new();
        public event Action<Collider> onTriggerExit;
        
        protected event Action OnBeforeProcessQueue;
    
        protected override void Awake()
        {
            base.Awake();
            collider.isTrigger = true;
            StartCoroutine(TriggerRoutine());
        }
        
        private IEnumerator TriggerRoutine()
        {
            while (true)
            {
                ProcessQueue();
                yield return new WaitForFixedUpdate();
            }
        }
        
        private void ProcessQueue()
        {
            OnBeforeProcessQueue?.Invoke();
            while (onTriggerEnterQueue.Count > 0)
            {
                var col = onTriggerEnterQueue.Dequeue();
                if(col == null) continue;
                Debugger.LogInfo($"TriggerEnter dequeue {col}");
                onTriggerEnter?.Invoke(col);
            }
    
            while (onTriggerExitQueue.Count > 0)
            {
                var col = onTriggerExitQueue.Dequeue();
                if(col == null) continue;
                Debugger.LogInfo($"TriggerExit dequeue {col}");
                onTriggerExit?.Invoke(col);
            }
        }
    
        protected virtual void OnTriggerEnter(Collider other)
        {
            collidersInside.Add(other);
            onTriggerEnterQueue.Enqueue(other);
        }
    
        protected virtual void OnTriggerExit(Collider other)
        {
            collidersInside.Remove(other);
            onTriggerExitQueue.Enqueue(other);
        }
    }
}
