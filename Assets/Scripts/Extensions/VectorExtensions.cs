using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PerceptionVR.Extensions
{
    public static class VectorExtensions
    {
        // Convert Vector3 of degrees to Vector3 of radians
        public static Vector3 Deg2Rad(this Vector3 degreeVector)
        {
            return new Vector3(degreeVector.x * Mathf.Deg2Rad, 
                               degreeVector.y * Mathf.Deg2Rad, 
                               degreeVector.z * Mathf.Deg2Rad);
        }
        
        // Convert Vector3 of radians to Vector3 of degrees
        public static Vector3 Rad2Deg(this Vector3 radianVector)
        {
            return new Vector3(radianVector.x * Mathf.Rad2Deg, 
                               radianVector.y * Mathf.Rad2Deg, 
                               radianVector.z * Mathf.Rad2Deg);
        }
        
        // Clamp Vector3 values to a min max range
        public static Vector3 Clamp(this Vector3 vector, float min, float max)
        {
            return new Vector3(Mathf.Clamp(vector.x, min, max), 
                               Mathf.Clamp(vector.y, min, max), 
                               Mathf.Clamp(vector.z, min, max));
        }
    }
}
