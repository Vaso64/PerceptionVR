using UnityEngine;


namespace PerceptionVR.Extensions
{
    public static class PlaneExtensions
    {
        // Converts plane to Vector4 plane representation (xyz: normal, w: distance)
        public static Vector4 ToVector4(this Plane plane) => new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
    }
}
