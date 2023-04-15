using PerceptionVR.Common;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace PerceptionVR.Portals
{
    public interface ITeleportableBehaviour : IMonoBehaviour
    {
        void OnCreateClone(CloneData cloneData, out IEnumerable<Type> preservedComponents);

        void OnEnterPortal(CloneData cloneData);
        
        void OnExitPortal(CloneData cloneData);
        
        void OnTeleport(CloneData cloneData);
    }
}