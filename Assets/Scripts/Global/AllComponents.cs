using UnityEngine;

namespace PerceptionVR.Global
{
    public static class ComponentTracking
    {
        public static ComponentTracker<Collider> allColliders = new();
    }
}