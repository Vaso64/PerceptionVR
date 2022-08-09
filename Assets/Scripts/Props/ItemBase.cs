using System;
using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Portal;
using UnityEngine;

namespace PerceptionVR.Props
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class ItemBase : MonoBehaviour, IGrabbable, ITeleportable
    {
        public Rigidbody rigidbody {get; protected set; }
    
        public Collider collider { get; protected set; }

        private void Start()
        {
            this.rigidbody = GetComponent<Rigidbody>();
            this.collider = GetComponent<Collider>();
        }
    }
}

