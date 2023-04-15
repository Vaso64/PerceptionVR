using UnityEngine;

namespace PerceptionVR.Extensions
{
    public static class BoundsExtensions
    {
        public static Plane[] GetPlanes(this Bounds bounds)
        {
            return new[]
            {
                new Plane(Vector3.up, bounds.max),
                new Plane(Vector3.down, bounds.min),
                new Plane(Vector3.right, bounds.max),
                new Plane(Vector3.left, bounds.min),
                new Plane(Vector3.forward, bounds.max),
                new Plane(Vector3.back, bounds.min)
            };
        }
    }
}