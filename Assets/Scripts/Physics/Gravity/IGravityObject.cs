using PerceptionVR.Common;
using UnityEngine;

namespace PerceptionVR.Physics
{
    public interface IGravityObject : IMonoBehaviour
    {
        Rigidbody rigidbody { get; }
        
        Quaternion gravityRotation { get; set; }
    }
}