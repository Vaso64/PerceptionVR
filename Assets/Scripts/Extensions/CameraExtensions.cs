using System.Linq;
using UnityEngine;

namespace PerceptionVR.Extensions
{
    public static class CameraExtension
    {
            // Converts a bounding box to screenspace rect
            public static Rect WorldToScreenBounds(this Camera camera, Bounds bounds)
            {
                var corners = GetWorldSpaceCorners(camera, bounds);

                // Project corners to screen space
                for (var i = 0; i < 8; i++)
                    corners[i] = camera.WorldToScreenPoint(corners[i], Camera.MonoOrStereoscopicEye.Mono);

                return new Rect
                {
                    xMin = corners.Min(corner => corner.x),
                    xMax = corners.Max(corner => corner.x),
                    yMin = corners.Min(corner => corner.y),
                    yMax = corners.Max(corner => corner.y)
                };
            }

            public static Rect WorldToViewportBounds(this Camera camera, Bounds bounds)
            {
                var corners = GetWorldSpaceCorners(camera, bounds);

                for (var i = 0; i < 8; i++)
                    corners[i] = camera.WorldToViewportPoint(corners[i], Camera.MonoOrStereoscopicEye.Mono);

                return new Rect
                {
                    xMin = corners.Min(corner => corner.x),
                    xMax = corners.Max(corner => corner.x),
                    yMin = corners.Min(corner => corner.y),
                    yMax = corners.Max(corner => corner.y)
                };
            }

            public static Vector3[] GetWorldSpaceCorners(Camera camera, Bounds b)
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
                
                for (var i = 0; i < 8; i++)
                {
                    // Shift corners from behind the projection plane to infront
                    // Note: the rect still fucks up if all corners are behind the camera.
                    //       maybe cap the points to screen dimmensions?
                    var dot = Vector3.Dot(camera.transform.forward, camera.transform.position - corners[i]);
                    corners[i] += dot >= 0 ? camera.transform.forward * (dot + 0.001f) : Vector3.zero;
                }
                
                return corners;
            }
            
            public static void SetNearPlane(this Camera camera, Plane plane, float offset = 0f)
            { 
                plane.distance += offset;
                var planePos = plane.ClosestPointOnPlane(camera.transform.position);
                var dotProduct = Vector3.Dot(plane.normal, planePos - camera.transform.position);

                // Skip if plane is behind the camera's near plane 
                if (dotProduct + camera.nearClipPlane > 0) // TODO: Fixed offsetting will probably break portal's which are scaled down too much
                    return;
                Vector3 cameraSpacePos = camera.worldToCameraMatrix.MultiplyPoint(planePos);
                Vector3 cameraSpaceNormal = camera.worldToCameraMatrix.MultiplyVector(plane.normal) * Mathf.Sign(dotProduct);
                float cameraSpaceDistance = -Vector3.Dot(cameraSpacePos, cameraSpaceNormal);
                camera.projectionMatrix = camera.CalculateObliqueMatrix(new Vector4(cameraSpaceNormal.x, cameraSpaceNormal.y, cameraSpaceNormal.z, cameraSpaceDistance));
            }


            public static void SetScissorRect(this Camera camera, Rect rect)
            {
                var pm = camera.projectionMatrix;
                var cameraRect = camera.rect;
                pm.m02 = (2f * rect.x + rect.width - 1f) / rect.width;
                pm.m12 = (2f * rect.y + rect.height - 1f) / rect.height;
                pm.m00 *= cameraRect.width / rect.width;
                pm.m11 *= cameraRect.height / rect.height;
                camera.projectionMatrix = pm;
                camera.rect = rect;
            }
    }
}
