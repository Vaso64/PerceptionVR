using System;
using PerceptionVR.Physics;
using PerceptionVR.Portals;
using UnityEngine;

namespace PerceptionVR.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(TeleportableObject))]
    public class PlayerBase: MonoBehaviourBase
    {
        public Action<TeleportData> OnTeleport { get; set; }
        public bool manualTeleport { get; set; }
        
        protected PhysicsObject playerPhysicsObject;
        
        public new Rigidbody rigidbody {get; private set; }

        protected virtual void Awake()
        {
            playerPhysicsObject = GetComponent<PhysicsObject>();
        }
    }
}
