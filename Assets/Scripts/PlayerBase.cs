using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PerceptionVR.Portal;

namespace PerceptionVR.Player
{
    public class PlayerBase : MonoBehaviour
    {
        List<PortalBase> portalList;
        // Start is called before the first frame update
        protected virtual void Start()
        {
            portalList = new List<PortalBase>();
            portalList.AddRange(FindObjectsOfType<PortalBase>());
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // Render all portals
            foreach(var portal in portalList)
                portal.RenderFromPose(new Pose(transform.position, transform.rotation));
        }
    }
}


