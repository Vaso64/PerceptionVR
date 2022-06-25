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

                Vector3[] screenspaceCorners = new Vector3[8];
                for (int i = 0; i < 8; i++)
                {
                    // Shift corners from behind the projection plane to infront
                    float dot = Vector3.Dot(camera.transform.forward, camera.transform.position - corners[i]);
                    corners[i] += dot >= 0 ? camera.transform.forward * (dot + 0.001f) : Vector3.zero;

                    // Project point
                    screenspaceCorners[i] = camera.WorldToScreenPoint(corners[i]);
                }

                // Note: the rect still fucks up if all corners are behind the camera.
                //       maybe cap the points to screen dimmensions?

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
