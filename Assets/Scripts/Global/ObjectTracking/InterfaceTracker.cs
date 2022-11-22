using System.Linq;
using UnityEngine;

namespace PerceptionVR.Global
{
    public class InterfaceTracker<T> : ObjectTrackerBase<T>
    {
        public InterfaceTracker()
        {
            // Store all interfaces T in scene
            var interfaces = Object.FindObjectsOfType<MonoBehaviour>().OfType<T>();
            allObjects.AddRange(interfaces);
        }
    }
}