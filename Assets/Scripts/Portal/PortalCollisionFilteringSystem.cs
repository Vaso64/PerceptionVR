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
        [SerializeField] public ColliderGroup largeGroup;
        [SerializeField] public ColliderGroup frontGroup;
        [SerializeField] public ColliderGroup passingGroup;
        [SerializeField] public ColliderGroup insideGroup;

        private Portal portal;
        private PortalVicinity vicinity;
        
        private void Start()
        {
            portal = GetComponentInParent<Portal>();
            vicinity = GetComponent<PortalVicinity>();

            largeGroup.debugName = $"largeGroup_{portal.transform.name}";
            frontGroup.debugName   = $"frontGroup_{portal.transform.name}";
            passingGroup.debugName = $"passingGroup_{portal.transform.name}";
            insideGroup.debugName  = $"insideGroup_{portal.transform.name}";
            
            // Object in front of the portal interact only with outside and passing group (ignores inside group)
            frontGroup.SetFilter(ColliderGroup.FilterMode.Exclude, new[] { insideGroup });
            
            // Object passing through portals interact only with inside and front group (ignores large group)
            passingGroup.SetFilter(ColliderGroup.FilterMode.Exclude, new[] { largeGroup });

            // Object inside the portal interact only with passing group (ignores front and large group)
            insideGroup.SetFilter(ColliderGroup.FilterMode.Exclude, new[] { frontGroup, largeGroup });

            vicinity.OnOutsideToLarge += OnOutsideToLarge;
            vicinity.OnLargeToOutside += OnLargeToOutside;
            vicinity.OnLargeToFront   += OnLargeToFront;
            vicinity.OnFrontToLarge   += OnFrontToLarge;
            vicinity.OnFrontToPass    += OnFrontToPass;
            vicinity.OnPassToFront    += OnPassToFront;
            vicinity.OnPassToInside   += PassToInside;
            vicinity.OnInsideToPass   += InsideToPass;
            vicinity.OnCloneIn        += OnCloneIn;
        }
        
        
        private void OnLargeToOutside(Collider other)
        {
            largeGroup.Remove(other);
        }
        
        private void OnOutsideToLarge(Collider other)
        {
            largeGroup.Add(other);
        }


        private void OnLargeToFront(Collider other)
        {
            largeGroup.Remove(other);
            frontGroup.Add(other);
        }
        
        private void OnFrontToLarge(Collider other)
        {
            frontGroup.Remove(other);
            largeGroup.Add(other);
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
        
        private void OnCloneIn(GameObject clone)
        {
            insideGroup.AddRange(clone.GetComponentsInChildren<Collider>());
        }
    }
}