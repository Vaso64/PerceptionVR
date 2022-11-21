using UnityEngine;

namespace PerceptionVR.Common
{
    public interface IGrabbable : IMonoBehaviour
    {
        Collider collider { get; }
        
        Rigidbody rigidbody { get; }
        
        void OnRelease() {}

        void OnGrab() {}
    }
}