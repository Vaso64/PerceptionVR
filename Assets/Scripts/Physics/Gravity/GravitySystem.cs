using System.Linq;
using PerceptionVR.Global;
using UnityEngine;

namespace PerceptionVR.Physics
{
    public class GravitySystem : MonoBehaviour
    {
        [SerializeField] private float gravityStrength = 9.81f;

        private void FixedUpdate()
        {
            foreach (var gravityObject in ComponentTracking.allGravityObjects.Where(x => x.rigidbody.useGravity))
                gravityObject.rigidbody.AddForce(gravityObject.gravityDirection * Vector3.down * gravityStrength, ForceMode.Acceleration);
        }
    }
}