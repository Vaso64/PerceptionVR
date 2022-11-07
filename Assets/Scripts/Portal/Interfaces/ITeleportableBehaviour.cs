using PerceptionVR.Common;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace PerceptionVR.Portal
{
    public interface ITeleportableBehaviour : IMonoBehaviour
    {
        void OnCreateClone(GameObject clone, out IEnumerable<Type> preservedComponents);

        void TransferBehaviour(GameObject from, GameObject to);
    }
}