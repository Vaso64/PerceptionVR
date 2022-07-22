using System;
using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Portal;
using PerceptionVR.Props;
using PerceptionVR.Portal;
using UnityEngine;

namespace PerceptionVR.Props
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Item : MonoBehaviour, IGrabbable, ITeleportable
    {
        public Rigidbody rigidbody {get; protected set; }
    
        public Collider collider { get; protected set; }

        private void Start()
        {
            this.rigidbody = GetComponent<Rigidbody>();
            this.collider = GetComponent<Collider>();
        }
    
        public void OnGrab()
        {
            Debug.Log(transform.name + " grabbed");
        }
    
        public void OnRelease()
        {
            Debug.Log(transform.name + " released");
        }
    
        public void OnTeleport(TeleportData teleportData)
        {
            Debug.Log(transform.name + " teleported");
        }
    }
}

