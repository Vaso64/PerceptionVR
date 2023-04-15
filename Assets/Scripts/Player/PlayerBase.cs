using System;
using PerceptionVR.Physics;
using PerceptionVR.Portals;
using UnityEngine;
using UnityEngine.Serialization;

namespace PerceptionVR.Player
{
    [RequireComponent(typeof(PhysicsObject))]
    [RequireComponent(typeof(TeleportableObject))]
    public class PlayerBase: MonoBehaviourBase
    {
        [FormerlySerializedAs("teleportableObject")] public TeleportableObject playerTeleportableObject;
        [FormerlySerializedAs("physicsObject")] public PhysicsObject playerPhysicsObject;

        protected virtual void Awake()
        {
            playerTeleportableObject = GetComponent<TeleportableObject>();
            playerPhysicsObject = GetComponent<PhysicsObject>();
        }
    }
}
