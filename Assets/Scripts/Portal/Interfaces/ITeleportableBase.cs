using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace PerceptionVR.Portal
{
    public interface ITeleportableBase
    {
        IEnumerable<Type> GetPreservedComponents() => Enumerable.Empty<Type>();

        void OnCreateClone(GameObject subClone) { }

        Transform transform { get; }
    }
}