using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private Vector3 previousPosition;
        

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
            var currentPosition = targetTeleportableBase.transform.position;
            var currentPortalSide = targetPortal.portalPlane.GetSide(currentPosition);

            // Target entered the portal
            if (currentPortalSide && !previousPortalSide && PassedThroughPortal(previousPosition, currentPosition))
                OnEnterPortal?.Invoke();
            
            // Target exited the portal
            else if (!currentPortalSide && previousPortalSide && PassedThroughPortal(previousPosition, currentPosition))
                OnExitPortal?.Invoke();
            
            previousPortalSide = currentPortalSide;
            previousPosition = currentPosition;
        }
        
        // Check if raycast between old and new position intersects portal collider
        private bool PassedThroughPortal(Vector3 oldPosition, Vector3 newPosition)
        {
            var direction = newPosition - oldPosition;
            var origin = oldPosition - direction.normalized;
            var ray = new Ray(origin, direction);
            var hits = Physics.RaycastAll(ray, Vector3.Distance(origin, newPosition));
            return hits.Any(hit => hit.collider == targetPortal.portalCollider);
        } 



        // Mimic target movement by rigidbody
        private IEnumerator TrackingCoroutine(Rigidbody trackedRigidbody, IPortal portal)
        {
            var cloneRigidbody = GetComponent<Rigidbody>();
            while (true)
            {
                var pose = trackedRigidbody.transform.GetPose();
                pose.position += trackedRigidbody.velocity * Time.fixedDeltaTime;
                pose.rotation *= Quaternion.Euler(trackedRigidbody.angularVelocity * Time.fixedDeltaTime);
                cloneRigidbody.VelocityPosition(portal.PairPose(pose));
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