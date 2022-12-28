using System;
using UnityEngine;


namespace PerceptionVR.Extensions
{
    public static class TransformExtension
    {
        public static Pose GetPose(this Transform transform) => new Pose(transform.position, transform.rotation);
        
        public static void SetPose(this Transform transform, Pose pose) => transform.SetPositionAndRotation(pose.position, pose.rotation);
        
        public static Pose GetLocalPose(this Transform transform) => new Pose(transform.localPosition, transform.localRotation);

        public static void SetLocalPose(this Transform transform, Pose pose) { transform.localPosition = pose.position; transform.localRotation = pose.rotation; }

        public static string GetPath(this Transform transform)
        {
            if(!transform) return String.Empty;
            return GetPath(transform.parent) + '/' + transform.name;
        }
        
        public static string GetPath(this Transform transform, Transform relativeTo)
        {
            if(!transform || transform == relativeTo) return String.Empty;
            if(transform.parent == relativeTo) return transform.name;
            return GetPath(transform.parent, relativeTo) + '/' + transform.name;
        }
    }
}
