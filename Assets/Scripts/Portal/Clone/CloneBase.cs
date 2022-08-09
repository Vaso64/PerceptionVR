using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using PerceptionVR.Extensions;

namespace PerceptionVR.Portal
{
    public class CloneBase : MonoBehaviour, ITeleportableBase
    {
        public IEnumerable<Type> GetPreservedComponents() => targetTeleportableBase.GetPreservedComponents();
        public void OnCreateClone(GameObject clone) => targetTeleportableBase.OnCreateClone(clone);
        
        
        public event Action OnEnterPortal;
        
        public event Action OnExitPortal;
        
        
        protected IPortal targetPortal;
        
        private ITeleportableBase targetTeleportableBase;

        private Coroutine trackingCoroutine;
        
        private bool previousPortalSide;
        

        // Set tracking target
        protected void Track(ITeleportableBase teleportableBase, IPortal portal)
        {
            this.targetPortal = portal;
            this.targetTeleportableBase = teleportableBase;

            // Stop old coroutine if it's running
            if (trackingCoroutine != null)
                StopCoroutine(trackingCoroutine);

            // Try get rigidbody from teleportable
            var targetRigidbody = teleportableBase.transform.GetComponent<Rigidbody>();

            // Start tracking coroutine
            trackingCoroutine = targetRigidbody ? 
                StartCoroutine(TrackingCoroutine(targetRigidbody, portal)) :  
                StartCoroutine(TrackingCoroutine(teleportableBase.transform, portal));
            
            // Set previous portal side
            previousPortalSide = portal.portalPlane.GetSide(targetTeleportableBase.transform.position);
            
            // Notify target
            teleportableBase.OnCreateClone(gameObject);
            
            //Debug.Log(transform.name + " is tracking " + teleportableBase.transform.name);
        }
        
        // Track target position
        private void Update()
        {
            var currentPortalSide = targetPortal.portalPlane.GetSide(targetTeleportableBase.transform.position);
            
            

            // Target entered the portal
            if (currentPortalSide && !previousPortalSide && targetPortal.teleportableBounds.Contains(targetTeleportableBase.transform.position))
                OnEnterPortal?.Invoke();
            
            // Target exited the portal
            else if (!currentPortalSide && previousPortalSide && targetPortal.teleportableBounds.Contains(targetTeleportableBase.transform.position))
                OnExitPortal?.Invoke();
            
            previousPortalSide = currentPortalSide;
        }
        
        
        // Mimic target movement by rigidbody
        private IEnumerator TrackingCoroutine(Rigidbody trackedRigidbody, IPortal portal)
        {
            var cloneRigidbody = GetComponent<Rigidbody>();
            while (true)
            {
                cloneRigidbody.VelocityPosition(portal.PairPose(trackedRigidbody.transform.GetPose()));
                yield return new WaitForFixedUpdate();
            }
        }



        // Mimic target movemnet by transform
        private IEnumerator TrackingCoroutine(Transform trackedTransform, IPortal portal)
        {
            while (true)
            {
                transform.SetPose(portal.PairPose(trackedTransform.GetPose()));
                yield return null;
            }
        }
    }
}