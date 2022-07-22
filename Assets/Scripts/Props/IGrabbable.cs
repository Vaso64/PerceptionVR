using UnityEngine;

namespace PerceptionVR.Props
{
    public interface IGrabbable
    {
        Transform transform { get; }
        
        Collider collider { get; }
        
        Rigidbody rigidbody { get; }
        
        void OnRelease();

        void OnGrab();
    }
}