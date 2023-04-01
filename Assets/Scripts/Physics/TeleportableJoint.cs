using PerceptionVR.Portals;
using UnityEngine;

namespace PerceptionVR.Physics
{
    //[RequireComponent(typeof(Joint))]
    public class TeleportableJoint : MonoBehaviour
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
                    connectedTeleportableBody.OnTeleport += teleportData => teleportData.inPortal.Teleport(selfTeleportableBody);
                }

                else
                {
                    // Teleport connected body when self teleports
                    connectedTeleportableBody.manualTeleport = true;
                    selfTeleportableBody.OnTeleport += teleportData => teleportData.inPortal.Teleport(connectedTeleportableBody);
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
                    connectedTeleportableBody.OnTeleport -= teleportData => teleportData.inPortal.Teleport(selfTeleportableBody);
                }

                else
                {
                    connectedTeleportableBody.manualTeleport = false;
                    selfTeleportableBody.OnTeleport -= teleportData => teleportData.inPortal.Teleport(connectedTeleportableBody);
                }
                    
                connectedTeleportableBody = null;
            }
            
            joint.connectedBody = null;
            isConnectedToBody = false;
        }
    }
}