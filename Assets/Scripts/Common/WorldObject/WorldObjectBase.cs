using System;
using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Global.Gravity;
using PerceptionVR.Portal;
using UnityEngine;

namespace PerceptionVR.Common
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class WorldObjectBase : MonoBehaviour, IGrabbable, ITeleportable, IGravityObject
    {
        public new Rigidbody rigidbody {get; private set; }
        public Quaternion gravityDirection { get; set; } = Quaternion.identity;

        public new Collider collider { get; private set; }

        private void Start()
        {
            this.rigidbody = GetComponent<Rigidbody>();
            this.collider = GetComponent<Collider>();
        }
    }
}

