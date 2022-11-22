using PerceptionVR.Global.Gravity;
using UnityEngine;

namespace PerceptionVR.Global
{
    public static class ComponentTracking
    {
        public static readonly ComponentTracker<Collider> allColliders = new();
        public static readonly InterfaceTracker<IGravityObject> allGravityObjects = new();
    }
}