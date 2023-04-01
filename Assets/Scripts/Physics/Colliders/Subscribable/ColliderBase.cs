using UnityEngine;

namespace PerceptionVR.Physics
{
    [RequireComponent(typeof(Collider))]
    public class ColliderBase: MonoBehaviourBase
    {
        [HideInInspector] public new Collider collider;
        
        protected virtual void Awake() => collider = GetComponent<Collider>();
    }
}