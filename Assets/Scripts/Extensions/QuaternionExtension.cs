using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PerceptionVR.Extensions
{
    public static class QuaternionExtension
    {
        // Clamp eulerAngles to -180 to 180
        public static Vector3 GetClampedEulerAngles(this Quaternion quaternion)
        {
            var euler = quaternion.eulerAngles;
            return new Vector3(
                euler.x > 180 ? euler.x - 360 : euler.x,
                euler.y > 180 ? euler.y - 360 : euler.y,
                euler.z > 180 ? euler.z - 360 : euler.z
            );
        }
        
        public static bool IsNaN(this Quaternion quaternion) => quaternion.x == 0 && quaternion.y == 0 && quaternion.z == 0 && quaternion.w == 0;
    }
}
