using System;
using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using PerceptionVR.Physics;


namespace PerceptionVR.Portals
{
    public partial class Portal : MonoBehaviour
    {
        [SerializeField] private Portal startingPortalPair;

        public Collider portalCollider { get; private set; }

        public float scaleRatio { get; private set; } = 1;

        public Plane portalPlane => new(transform.forward, transform.position);
        

        private void Awake()
        {
            portalCollider = GetComponent<Collider>();
        }

        private void Start()
        {
            // Portal pair checks
            if (startingPortalPair != null)
                SetPortalPair(this, startingPortalPair);
        }

        public void Teleport(ITeleportable teleportable)
        {
            // Teleport the object
            var pairPose = this.PairPose(teleportable.transform.GetPose(), out var rotationDelta);
            teleportable.transform.SetPose(pairPose);
            
            // Translate velocity
            foreach (var teleportedRB in teleportable.transform.GetComponentsInChildren<Rigidbody>())
                teleportedRB.velocity = rotationDelta * teleportedRB.velocity;

            // Reorient the object
            foreach (var gravityObject in teleportable.transform.GetComponentsInChildren<IGravityObject>())
                gravityObject.gravityDirection = rotationDelta * gravityObject.gravityDirection;
            
            // Scale
            teleportable.transform.localScale *= 1 / scaleRatio;

            // Notify
            var teleportData = new TeleportData
            {
                teleportable = teleportable,
                inPortal = this,
                outPortal = portalPair,
                rotationDelta = rotationDelta
            };
            
            GlobalEvents.OnTeleport?.Invoke(teleportData);
            teleportable.OnTeleport?.Invoke(teleportData);
        }
    }
}
