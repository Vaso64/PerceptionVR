using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerceptionVR.Portal
{
    [RequireComponent(typeof(PortalBase))]
    public class PortalRenderer : MonoBehaviour
    {
        [SerializeField]
        private Renderer portalRend;

        private PortalBase portalBase;
        
        private Camera portalCamera;
        
        void Start()
        {
            portalBase = GetComponent<PortalBase>();
            portalCamera = GetComponentInChildren<Camera>();
            portalCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            portalRend.material.mainTexture = portalCamera.targetTexture;
        }

        public void RenderFromPose(Pose pose)
        {
            var pairPose = portalBase.CalculatePairPose(pose);

            portalCamera.transform.position = pairPose.position;
            portalCamera.transform.rotation = pairPose.rotation;

            // Render
            portalCamera.Render();
        }

    }
}



