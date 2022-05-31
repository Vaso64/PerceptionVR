using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerceptionVR.Portal
{
    public class PortalBase : MonoBehaviour
    {
        public Transform portalPair;

        private Camera portalCamera;
        private Material portalMaterial;

        protected virtual void Start()
        {
            portalCamera = GetComponentInChildren<Camera>();
            portalCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            gameObject.GetComponent<Renderer>().material.mainTexture = portalCamera.targetTexture;
        }


        public void RenderFromPose(Pose pose)
        {
            Vector3 positionDelta = pose.position - transform.position;
            Quaternion portalRotationDelta = portalPair.rotation * Quaternion.Inverse(transform.rotation) * Quaternion.Euler(0, 180, 0);

            portalCamera.transform.position = portalPair.position + portalRotationDelta* positionDelta;
            portalCamera.transform.rotation = portalRotationDelta * pose.rotation;

            portalCamera.Render();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter Portal");
        }
    }
}


