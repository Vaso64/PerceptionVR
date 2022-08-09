using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PerceptionVR.Portal
{
    public interface ISubTeleportable : ITeleportableBase
    {
        // Swap custom behaviour of subTeleportable
        void TransferBehaviour(ISubTeleportable from, ISubTeleportable to) {}
    }
}