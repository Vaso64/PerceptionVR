using System;
using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using PerceptionVR.Physics;


namespace PerceptionVR.Portals
{
    public partial class Portal: MonoBehaviourBase
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

        public void Teleport(TeleportableObject teleportableObject)
        {
            // Teleport the object
            var pairPose = this.PairPose(teleportableObject.transform.GetPose(), out var rotationDelta);
            teleportableObject.transform.SetPose(pairPose);
            
            // Try get PhysicsObject
            if (teleportableObject.TryGetComponent(out PhysicsObject physicsObject))
            {
                // Reorient velocity
                physicsObject.rigidbody.velocity = rotationDelta * physicsObject.rigidbody.velocity;
                
                // Reorient gravity
                physicsObject.gravityRotation = rotationDelta * physicsObject.gravityRotation;
            }

            // Scale
            teleportableObject.transform.localScale *= 1 / scaleRatio;

            // Notify
            var teleportData = new TeleportData
            {
                teleportableObject = teleportableObject,
                inPortal = this,
                outPortal = portalPair,
                rotationDelta = rotationDelta
            };
            
            GlobalEvents.OnTeleport?.Invoke(teleportData);
            teleportableObject.OnTeleport?.Invoke(teleportData);
        }
    }
}
