using System;
using PerceptionVR.Physics;
using PerceptionVR.Portal;
using UnityEngine;

namespace PerceptionVR.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerBase : MonoBehaviour, ITeleportable, IGravityObject
    {
        public Action<TeleportData> OnTeleport { get; set; }
        public bool manualTeleport { get; set; }
        
        public new Rigidbody rigidbody {get; private set; }
        public Quaternion gravityDirection { get; set; } = Quaternion.identity;

        protected virtual void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }
    }
}
