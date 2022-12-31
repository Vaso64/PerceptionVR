using PerceptionVR.Physics;
using PerceptionVR.Portal;
using UnityEngine;

namespace PerceptionVR.Common
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class WorldObjectBase : MonoBehaviour, IGrabbable, ITeleportable, IGravityObject
    {
        [SerializeField] private Direction startingGravityDirection = Direction.Down;
        
        public new Rigidbody rigidbody {get; private set; }
        public Quaternion gravityDirection { get; set; } = Quaternion.identity;

        public new Collider collider { get; private set; }

        private void Start()
        {
            this.rigidbody = GetComponent<Rigidbody>();
            this.collider = GetComponent<Collider>();
            this.gravityDirection = Quaternion.FromToRotation(Vector3.down, startingGravityDirection.ToVector3());
        }
    }
}

