using PerceptionVR.Portals;
using UnityEngine;

namespace PerceptionVR.Physics
{
    //[RequireComponent(typeof(Joint))]
    public class TeleportableJoint: MonoBehaviourBase
    {
        public bool isConnectedToBody { get; private set; }
        public ConfigurableJoint joint;
        private TeleportableObject selfTeleportableBody;
        private TeleportableObject connectedTeleportableBody;

        private void Awake()
        {
            selfTeleportableBody = GetComponentInParent<TeleportableObject>();
            if(joint.connectedBody != null)
                SetConnectedBody(joint.connectedBody);
        } 

        public void SetConnectedBody(Rigidbody body)
        {
            // Release old body
            ReleaseConnectedBody();
                
            if (body.TryGetComponent(out connectedTeleportableBody))
            {
                if (joint.swapBodies)
                {
                    // Teleport self when connected body teleports
                    selfTeleportableBody.manualTeleport = true;
                    connectedTeleportableBody.OnTeleport += TeleportSelf;
                }

                else
                {
                    // Teleport connected body when self teleports
                    connectedTeleportableBody.manualTeleport = true;
                    selfTeleportableBody.OnTeleport += TeleportConnected;
                }
            }

            joint.connectedBody = body;
            isConnectedToBody = true;
        }
        
        public void ReleaseConnectedBody()
        {
            if (connectedTeleportableBody != null)
            {
                if (joint.swapBodies)
                {
                    selfTeleportableBody.manualTeleport = false;
                    connectedTeleportableBody.OnTeleport -= TeleportSelf;
                }

                else
                {
                    connectedTeleportableBody.manualTeleport = false;
                    selfTeleportableBody.OnTeleport -= TeleportConnected;
                }
                    
                connectedTeleportableBody = null;
            }
            
            joint.connectedBody = null;
            isConnectedToBody = false;
        }
        
        private void TeleportConnected(TeleportData teleportData) => teleportData.inPortal.Teleport(connectedTeleportableBody);
        private void TeleportSelf(TeleportData teleportData) => teleportData.inPortal.Teleport(selfTeleportableBody);
    }
}