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
        [SerializeField] public ColliderGroup insideGroup;

        private Portal portal;
        private PortalVicinity vicinity;
        
        private void Start()
        {
            portal = GetComponentInParent<Portal>();
            vicinity = GetComponent<PortalVicinity>();

            frontGroup.debugName   = $"frontGroup_{portal.transform.name}";
            passingGroup.debugName = $"passingGroup_{portal.transform.name}";
            insideGroup.debugName  = $"cloneGroup_{portal.transform.name}";
            
            // Object passing through portals interact only with inside and front group
            passingGroup.SetFilter(ColliderGroup.FilterMode.Include, new[] { insideGroup, frontGroup });

            // Object inside the portal interact only with passing group
            insideGroup.SetFilter(ColliderGroup.FilterMode.Include, new[] { passingGroup });

            vicinity.OnOutsideToFront += OnOutsideToFront;
            vicinity.OnFrontToOutside += OnFrontToOutside;
            vicinity.OnFrontToPass    += OnFrontToPass;
            vicinity.OnPassToFront    += OnPassToFront;
            vicinity.OnPassToInside   += PassToInside;
            vicinity.OnInsideToPass   += InsideToPass;
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
        
        private void PassToInside(Collider other)
        {
            passingGroup.Remove(other);
            insideGroup.Add(other);
        }
        
        private void InsideToPass(Collider other)
        {
            if(!insideGroup.Remove(other))
                Debugger.LogWarning($"{other} entering portal from behind");
            passingGroup.Add(other);
        }
    }
}