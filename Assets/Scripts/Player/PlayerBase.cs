using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PerceptionVR.Portal;

namespace PerceptionVR.Player
{
    public class PlayerBase : MonoBehaviour
    {
        List<PortalRenderer> portalList;

        private Camera playerCamera;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            portalList = new List<PortalRenderer>();
            portalList.AddRange(FindObjectsOfType<PortalRenderer>());
            foreach(var cam in GetComponentsInChildren<Camera>())
                cam.nearClipPlane = 0.00001f;
            this.playerCamera = GetComponentInChildren<Camera>();
        }

        private void OnPreRender()
        {
            // Render all portals
            foreach(var portal in portalList)
                portal.RenderFromPose(new Pose(transform.position, transform.rotation));
        }
    }
}


