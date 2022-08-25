using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SubscribableCollider : MonoBehaviour
{
    public Action<Collider> onTriggerEnter;
    
    public Action<Collider> onTriggerExit;
    
    public Action<Collision> onCollisionEnter;
    
    public Action<Collision> onCollisionExit;

    private void OnTriggerEnter(Collider other) => onTriggerEnter?.Invoke(other);
    
    private void OnTriggerExit(Collider other) => onTriggerExit?.Invoke(other);

    private void OnCollisionEnter(Collision collision) => onCollisionEnter?.Invoke(collision);

    private void OnCollisionExit(Collision collision) => onCollisionExit?.Invoke(collision);
}
