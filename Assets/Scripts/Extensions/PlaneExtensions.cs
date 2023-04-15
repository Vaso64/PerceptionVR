using UnityEngine;


namespace PerceptionVR.Extensions
{
    public static class PlaneExtensions
    {
        // Converts plane to Vector4 plane representation (xyz: normal, w: distance)
        public static Vector4 ToVector4(this Plane plane) => new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
        
        // Get point between two points where a plane intersects
        public static Vector3 TwoPointIntersection(this Plane plane, Vector3 p1, Vector3 p2)
        {
            var p1dot = plane.GetDistanceToPoint(p1);
            var p2dot = plane.GetDistanceToPoint(p2);
            return p1 + (p2 - p1) * (p1dot / (p1dot - p2dot));
        }
    }
}
