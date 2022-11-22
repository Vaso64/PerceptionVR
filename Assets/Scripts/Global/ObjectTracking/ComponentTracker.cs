using UnityEngine;

namespace PerceptionVR.Global
{
    public class ComponentTracker<T> : ObjectTrackerBase<T> where T : Component
    {
        public ComponentTracker()
        {
            // Store all components T in scene
            var components = Object.FindObjectsOfType<T>();
            allObjects.AddRange(components);
        }
    }
}