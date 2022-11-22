using PerceptionVR.Common;
using UnityEngine;

namespace PerceptionVR.Global.Gravity
{
    public interface IGravityObject : IMonoBehaviour
    {
        Rigidbody rigidbody { get; }
        
        Quaternion gravityDirection { get; set; }
    }
}