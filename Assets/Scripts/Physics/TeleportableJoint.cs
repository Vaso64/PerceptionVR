using System;
using System.Collections.Generic;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using PerceptionVR.Portals;
using UnityEngine;

namespace PerceptionVR.Physics
{
    [RequireComponent(typeof(Joint))]
    public class TeleportableJoint : MonoBehaviour
    {
        public bool isConnectedToBody { get; private set; }
        private Joint joint;
        private ITeleportable connectedBodyTeleportable;

        private void Awake() => GetComponentInParent<ITeleportable>().OnTeleport += OnTeleportCallback;

        public T SetConnectedBody<T>(Rigidbody body) where T : Joint
        {
            // Release old body
            ReleaseConnectedBody();

            joint = gameObject.AddComponentNotify<T>();
            connectedBodyTeleportable = body.GetComponentInParent<ITeleportable>();
            connectedBodyTeleportable.manualTeleport = true;
            
            joint.connectedBody = body;
            
            isConnectedToBody = true;

            return (T)joint;
        }
        
        public void ReleaseConnectedBody()
        {
            if (connectedBodyTeleportable != null)
            {
                connectedBodyTeleportable.manualTeleport = false;
                connectedBodyTeleportable = null;
            }
            
            if(joint != null)
                GameObjectUtility.DestroyComponentNotify(joint);
            
            isConnectedToBody = false;
        }

        private void OnTeleportCallback(TeleportData teleportData)
        {
            if(connectedBodyTeleportable != null)
                teleportData.inPortal.Teleport(connectedBodyTeleportable);
            
        }
    }
}