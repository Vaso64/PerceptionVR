using System;
using UnityEngine;
using System.Linq;
using PerceptionVR.Debug;


namespace PerceptionVR.Portal
{
    // Tracks if object passed through portal
    public class TrackedCloneBase<T> : CloneBase<T> where T : Component
    {
        public event Action OnEnterPortal;
        public event Action OnExitPortal;
        
        [SerializeField]private Vector3 currentPosition;
        [SerializeField]private Vector3 previousPosition;
        [SerializeField]private bool currentPortalSide;
        [SerializeField]private bool previousPortalSide;
        
        
        // Set tracking target
        public override void Track(T target, IPortal throughPortal)
        {
            currentPosition = target.transform.position;
            previousPosition = currentPosition;
            currentPortalSide = throughPortal.portalPlane.GetSide(currentPosition);
            previousPortalSide = currentPortalSide;
            
            base.Track(target, throughPortal);
        }
        
        // Track target position
        protected virtual void Update()
        {
            if (!targetSet)
                return;
            
            currentPosition = target.transform.position;
            currentPortalSide = throughPortal.portalPlane.GetSide(currentPosition);
    
            // Target entered the portal
            if (currentPortalSide && !previousPortalSide && PassedThroughPortal(previousPosition, currentPosition))
            {
                Debugger.LogInfo($"{target} entered portal {throughPortal.transform}");
                OnEnterPortal?.Invoke();
            }
                
            
            // Target exited the portal
            else if (!currentPortalSide && previousPortalSide && PassedThroughPortal(previousPosition, currentPosition))
            {
                Debugger.LogInfo($"{target} exited portal {throughPortal.transform}");
                OnExitPortal?.Invoke();
            }

            previousPortalSide = currentPortalSide;
            previousPosition = currentPosition;
        }
        
        // Check if raycast between old and new position intersects portal collider
        private bool PassedThroughPortal(Vector3 oldPosition, Vector3 newPosition)
        {
            var direction = newPosition - oldPosition;
            var origin = oldPosition - direction.normalized;
            var ray = new Ray(origin, direction);
            var hits = UnityEngine.Physics.RaycastAll(ray, Vector3.Distance(origin, newPosition));
            var result = hits.Any(hit => hit.collider == throughPortal.portalCollider);
            if(!result)
                Debugger.LogInfo($"{transform} passed through portal failed {oldPosition} -> {newPosition}");
            return result;
        } 
    }
}