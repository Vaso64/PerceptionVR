using System;
using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Common;
using UnityEngine;

public class SubscribableTrigger : ColliderBase, IEnumerable<Collider>
{
    public IEnumerator<Collider> GetEnumerator() => triggerArea.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    private readonly ICollection<Collider> triggerArea = new List<Collider>();

    private readonly Queue<Collider> onTriggerEnterQueue = new();
    public event Action<Collider> onTriggerEnter;
    
    private readonly Queue<Collider> onTriggerExitQueue = new();
    public event Action<Collider> onTriggerExit;

    private new void Awake()
    { 
        base.Awake();
        collider.isTrigger = true;
        StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        while (true)
        {
            while (onTriggerEnterQueue.Count > 0)
            {
                var col = onTriggerEnterQueue.Dequeue();
                //Dbg.LogInfo($"[SubscribableTrigger] OnTriggerEnter {col} in {name}");
                onTriggerEnter?.Invoke(col);
            }


            while (onTriggerExitQueue.Count > 0)
            {
                var col = onTriggerExitQueue.Dequeue();
                //Dbg.LogInfo($"[SubscribableTrigger] OnTriggerExit {col} in {name}");
                onTriggerExit?.Invoke(col);
            }
                
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerArea.Add(other);
        onTriggerEnterQueue.Enqueue(other);
    }

    private void OnTriggerExit(Collider other)
    {
        triggerArea.Remove(other);
        onTriggerExitQueue.Enqueue(other);
    }
}