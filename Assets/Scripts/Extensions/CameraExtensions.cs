using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PerceptionVR.Extensions
{
    public static class CameraExtension
    {
            // Converts a bounding box to screenspace rect
            public static Rect WorldToScreenBounds(this Camera camera, Bounds b)
            {
                Vector3[] corners = { b.center + new Vector3( b.extents.x,  b.extents.y,  b.extents.z),
                                    b.center + new Vector3( b.extents.x,  b.extents.y, -b.extents.z),
                                    b.center + new Vector3( b.extents.x, -b.extents.y,  b.extents.z),
                                    b.center + new Vector3( b.extents.x, -b.extents.y, -b.extents.z),
                                    b.center + new Vector3(-b.extents.x,  b.extents.y,  b.extents.z),
                                    b.center + new Vector3(-b.extents.x,  b.extents.y, -b.extents.z),
                                    b.center + new Vector3(-b.extents.x, -b.extents.y,  b.extents.z),
                                    b.center + new Vector3(-b.extents.x, -b.extents.y, -b.extents.z) 
                };

                Vector3[] screenspaceCorners = corners.Select(corner => camera.WorldToScreenPoint(corner)).ToArray();

                return new Rect() 
                {
                    xMin = screenspaceCorners.Min(corner => corner.x),
                    xMax = screenspaceCorners.Max(corner => corner.x),
                    yMin = screenspaceCorners.Min(corner => corner.y),
                    yMax = screenspaceCorners.Max(corner => corner.y),
                };
            }
    }
}
