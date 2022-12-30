using System;
using System.ComponentModel;
using PerceptionVR.Debug;
using UnityEngine;

namespace PerceptionVR.Portal
{
    // Portal pair section
    public partial class Portal
    {
        public event Action<Portal> OnPortalPairSet;
        public event Action OnPortalPairUnset;

        private Portal _portalPair;
        public Portal portalPair => _portalPair;


        private void OnValidate()
        {
            // Self as pair check
            if (startingPortalPair == this)
            {
                Debugger.LogError("Portal pair cannot be the same as the portal itself");
                startingPortalPair = null;
                return;
            }
            
            // Set pair other way around
            if (startingPortalPair != null && startingPortalPair.startingPortalPair != this)
                startingPortalPair.startingPortalPair = this;
        }


        public static void SetPortalPair(Portal portalA, Portal portalB)
        {
            // Skip if already set
            if(portalA._portalPair == portalB && portalB._portalPair == portalA) 
                return;
            
            // Unset previous pairs
            portalA.UnsetPortalPair();
            portalB.UnsetPortalPair();
            
            // Set new pairs
            portalA._portalPair = portalB;
            portalB._portalPair = portalA;
            portalA.OnPortalPairSet?.Invoke(portalB);
            portalB.OnPortalPairSet?.Invoke(portalA);
        }
        
        public void UnsetPortalPair()
        {
            if (_portalPair != null)
            {
                _portalPair._portalPair = null;
                _portalPair.OnPortalPairUnset?.Invoke();
            }
            
            _portalPair = null;
            OnPortalPairUnset?.Invoke();
        }
    }
}