using System;
using System.Collections.Generic;
using System.Linq;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;
using PerceptionVR.Physics;
using PerceptionVR.Debug;


namespace PerceptionVR.Portal
{

    public class PortalCollisionFilteringSystem : MonoBehaviour
    {
        [SerializeField] protected SubscribableSwapTrigger frontArea;
        [SerializeField] protected SubscribableSwapTrigger passingArea;
        [SerializeField] protected SubscribableSwapTrigger backArea;

        [SerializeField] protected ColliderGroup frontGroup;
        [SerializeField] protected ColliderGroup passingGroup;
        [SerializeField] protected ColliderGroup backGroup;
        [SerializeField] protected ColliderGroup cloneGroup;

        protected event Action<Collider> OnEnteredVicinity;
        protected event Action<Collider> OnExitedVicinity;

        protected IPortal portal;


        protected virtual void Start()
        {
            portal = GetComponentInParent<IPortal>();

            frontGroup.debugName = $"frontGroup_{portal.transform.name}";
            passingGroup.debugName = $"passingGroup_{portal.transform.name}";
            backGroup.debugName = $"backGroup_{portal.transform.name}";
            cloneGroup.debugName = $"cloneGroup_{portal.transform.name}";
            
            // Object passing through portals interact only with clone group and front group
            passingGroup.SetFilter(ColliderGroup.FilterMode.Include, new[] { cloneGroup, frontGroup });

            // Object inside the portal (clone group) interact only with passing group
            cloneGroup.SetFilter(ColliderGroup.FilterMode.Include, new[] { passingGroup });

            frontArea.onTriggerEnter += OnFrontAreaEnter;
            frontArea.onTriggerExit += OnFrontAreaExit;
            passingArea.onTriggerEnter += OnPassingAreaEnter;
            passingArea.onTriggerExit += OnPassingAreaExit;
            backArea.onTriggerEnter += OnBackAreaEnter;
            backArea.onTriggerExit += OnBackAreaExit;
        }


        private void OnFrontAreaEnter(Collider other)
        {
            // Object entered from passing area (exiting portal)
            if(passingGroup.Contains(other))
                return;
            
            // Object is in clone group
            if(cloneGroup.Contains(other))
                return;

            // Object entered from behind the portal
            if (backGroup.Contains(other))
            {
                backGroup.Remove(other);
                frontGroup.Add(other);
                OnEnteredVicinity?.Invoke(other);
                return;
            }

            // Object entered from outside
            frontGroup.Add(other);
            OnEnteredVicinity?.Invoke(other);
        }

        private void OnFrontAreaExit(Collider other)
        {
            // Object is in clone group (entering portal)
            if(cloneGroup.Contains(other))
                return;

            // Object exited to passing area 
            if (passingArea.Contains(other))
            {
                frontGroup.Remove(other);
                passingGroup.Add(other);
                return;
            }
            
            // Object is going behind the portal
            if (backArea.Contains(other))
            {
                frontGroup.Remove(other);
                backGroup.Add(other);
                OnExitedVicinity?.Invoke(other);
                return;
            }
            
            // Object exited to outside
            frontGroup.Remove(other);
            OnExitedVicinity?.Invoke(other);
        }
        
        private void OnPassingAreaEnter(Collider other)
        {
            // Object entered from back area (clone exiting portal)
            if (cloneGroup.Contains(other))
            {
                cloneGroup.Remove(other);
                passingGroup.Add(other);
                return;
            }

            // Object entered from front area (entering portal)
            if (frontGroup.Contains(other))
            {
                frontGroup.Remove(other);
                passingGroup.Add(other);
                return;
            }
            
            // Object is entering from behind the portal
            if (backGroup.Contains(other))
            {
                Debugger.LogWarning("Object is entering from behind the portal");
                return;
            }

            // Object already in passing group
            if (passingGroup.Contains(other))
            {
                Debugger.LogWarning($"{other} entered passing area but was already in passing group");
                return;
            }

            Debugger.LogWarning($"{other} entered passing area in an unexpected way");
        }
        
        private void OnPassingAreaExit(Collider other)
        {
            // Object exited to back area (entering portal)
            if (backArea.Contains(other))
            {
                passingGroup.Remove(other);
                cloneGroup.Add(other);
                return;
            }

            // Object exited to front area (exiting portal)
            if (frontArea.Contains(other))
            {
                passingGroup.Remove(other);
                frontGroup.Add(other);
                return;
            }

            Debugger.LogWarning($"{other} exited passing area in an unexpected way");
        }
        
        private void OnBackAreaEnter(Collider other)
        {
            // Object entered from passing area
            if (passingGroup.Contains(other))
                return;

            // Object is in clone group
            if (cloneGroup.Contains(other))
                return;
            
            // Object is both in front and back area
            if (frontGroup.Contains(other))
                return;

            // Object entered from outside area
            backGroup.Add(other);
        }
        
        private void OnBackAreaExit(Collider other)
        {
            // Object is in clone group
            if (cloneGroup.Contains(other))
                return;
            
            // Object exited through passing area (exiting portal)
            if (passingGroup.Contains(other))
                return;

            // Object behind the portal exited
            if (backGroup.Contains(other))
            {
                backGroup.Remove(other);
                return;
            }
            
            // Object exited to front area  
            if (frontGroup.Contains(other))
                return;

            Debugger.LogWarning($"{other} exited back area in an unexpected way");
        }
    }
}