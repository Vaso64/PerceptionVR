using UnityEngine;

namespace PerceptionVR.Common
{
    public interface IMonoBehaviour
    {
        Transform transform { get; }
        
        GameObject gameObject { get; }
    }
}