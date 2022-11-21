using UnityEngine;

namespace PerceptionVR.Common
{
    [RequireComponent(typeof(Collider))]
    public class ColliderBase : MonoBehaviour
    {
        [HideInInspector] public new Collider collider;
        
        protected virtual void Awake() => collider = GetComponent<Collider>();
    }
}