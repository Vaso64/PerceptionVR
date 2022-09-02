using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SubscribableCollider : MonoBehaviour, IEnumerable<Collider>
{
    public new Collider collider { get; private set; }
    
    public IEnumerator<Collider> GetEnumerator() => triggerArea.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly ICollection<Collider> triggerArea = new List<Collider>();

    public Action<Collider> onTriggerEnter;
    
    public Action<Collider> onTriggerExit;
    
    public Action<Collision> onCollisionEnter;
    
    public Action<Collision> onCollisionExit;


    private void Awake() => collider = GetComponent<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        triggerArea.Add(other);
        onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        triggerArea.Remove(other);
        onTriggerExit?.Invoke(other);
    }

    private void OnCollisionEnter(Collision collision) => onCollisionEnter?.Invoke(collision);

    private void OnCollisionExit(Collision collision) => onCollisionExit?.Invoke(collision);
}
