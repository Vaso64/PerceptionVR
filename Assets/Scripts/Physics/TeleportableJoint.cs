using System;
using System.Collections.Generic;
using PerceptionVR.Portal;
using UnityEngine;

namespace PerceptionVR.Physics
{
    [RequireComponent(typeof(Joint))]
    public class TeleportableJoint : MonoBehaviour
    {
        public Joint joint;
        private ITeleportable connectedBodyTeleportable;

        private void Awake()
        {
            joint = GetComponent<Joint>();
            GetComponent<ITeleportable>().OnTeleport += OnTeleportCallback;
        } 
        
        public void SetConnectedBody(Rigidbody body)
        {
            if (connectedBodyTeleportable != null)
                connectedBodyTeleportable.manualTeleport = false;
            
            joint.connectedBody = body;
            if (body != null)
            {
                connectedBodyTeleportable = body.GetComponentInParent<ITeleportable>();
                connectedBodyTeleportable.manualTeleport = true;
            }
            else
                connectedBodyTeleportable = null;
        }

        private void OnTeleportCallback(TeleportData teleportData)
        {
            if(connectedBodyTeleportable != null)
                teleportData.inPortal.Teleport(connectedBodyTeleportable);
        }
    }
}