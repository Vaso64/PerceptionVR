using UnityEngine;

namespace PerceptionVR.Common
{
    public interface IGrabbable
    {
        Transform transform { get; }
        
        Collider collider { get; }
        
        Rigidbody rigidbody { get; }
        
        void OnRelease() {}

        void OnGrab() {}
    }
}