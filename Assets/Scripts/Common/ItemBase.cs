using System;
using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Portal;
using UnityEngine;

namespace PerceptionVR.Common
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class ItemBase : MonoBehaviour, IGrabbable, ITeleportable
    {
        public new Rigidbody rigidbody {get; private set; }
    
        public new Collider collider { get; private set; }

        private void Start()
        {
            this.rigidbody = GetComponent<Rigidbody>();
            this.collider = GetComponent<Collider>();
        }
    }
}

