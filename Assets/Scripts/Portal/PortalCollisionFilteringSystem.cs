using System;
using System.Linq;
using UnityEngine;
using PerceptionVR.Physics;
using PerceptionVR.Debug;


namespace PerceptionVR.Portals
{
    [RequireComponent(typeof(PortalVicinity))]
    public class PortalCollisionFilteringSystem : MonoBehaviour
    {
        [SerializeField] public ColliderGroup frontGroup;
        [SerializeField] public ColliderGroup passingGroup;
        [SerializeField] public ColliderGroup cloneGroup;

        private Portal portal;
        private PortalVicinity vicinity;
        
        protected virtual void Start()
        {
            portal = GetComponentInParent<Portal>();
            vicinity = GetComponent<PortalVicinity>();

            frontGroup.debugName   = $"frontGroup_{portal.transform.name}";
            passingGroup.debugName = $"passingGroup_{portal.transform.name}";
            cloneGroup.debugName   = $"cloneGroup_{portal.transform.name}";
            
            // Object passing through portals interact only with clone group and front group
            passingGroup.SetFilter(ColliderGroup.FilterMode.Include, new[] { cloneGroup, frontGroup });

            // Object inside the portal (clone group) interact only with passing group
            cloneGroup.SetFilter(ColliderGroup.FilterMode.Include, new[] { passingGroup });

            vicinity.OnOutsideToFront += OnOutsideToFront;
            vicinity.OnFrontToOutside += OnFrontToOutside;
            vicinity.OnFrontToPass    += OnFrontToPass;
            vicinity.OnPassToFront    += OnPassToFront;
            vicinity.OnPassToBack     += OnPassToBack;
            vicinity.OnBackToPass     += OnBackToPass;
            vicinity.OnFrontToBack    += OnFrontToBack;
            vicinity.OnBackToFront    += OnBackToFront;
        }


        private void OnOutsideToFront(Collider other)
        {
            frontGroup.Add(other);
        }
        
        private void OnFrontToOutside(Collider other)
        {
            frontGroup.Remove(other);
        }
        
        private void OnFrontToPass(Collider other)
        {
            frontGroup.Remove(other);
            passingGroup.Add(other);
        }
        
        private void OnPassToFront(Collider other)
        {
            passingGroup.Remove(other);
            frontGroup.Add(other);
        }
        
        private void OnPassToBack(Collider other)
        {
            passingGroup.Remove(other);
            cloneGroup.Add(other);
        }
        
        private void OnBackToPass(Collider other)
        {
            if(!cloneGroup.Remove(other))
                Debugger.LogWarning($"{other} entering portal from behind");
            passingGroup.Add(other);
        }

        private void OnFrontToBack(Collider other)
        {
            if(cloneGroup.Contains(other))
                return;
            
            frontGroup.Remove(other);
        }
        
        private void OnBackToFront(Collider other)
        {
            if(cloneGroup.Contains(other))
                return;
            
            frontGroup.Add(other);
        }
    }
}