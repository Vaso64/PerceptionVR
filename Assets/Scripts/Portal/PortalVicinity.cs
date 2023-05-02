using System;
using System.Linq;
using PerceptionVR.Debug;
using PerceptionVR.Physics;
using UnityEngine;

namespace PerceptionVR.Portals
{
    public class PortalVicinity: MonoBehaviourBase
    {
        [SerializeField] public SubscribableSwapTrigger largeArea;
        [SerializeField] public SubscribableSwapTrigger frontArea;
        [SerializeField] public SubscribableSwapTrigger passingArea;
        [SerializeField] public SubscribableSwapTrigger insideArea;

        public event Action<Collider> OnOutsideToLarge;
        public event Action<Collider> OnLargeToOutside;
        
        public event Action<Collider> OnLargeToFront;
        public event Action<Collider> OnFrontToLarge;

        public event Action<Collider> OnFrontToPass;
        public event Action<Collider> OnPassToFront;
        
        public event Action<Collider> OnPassToInside;
        public event Action<Collider> OnInsideToPass;
        
        public       Action<GameObject> OnCloneIn; // Invoked by PortalCloneSystem


        private void Awake()
        {
            largeArea.onTriggerEnter   += OnLargeAreaEnter;
            largeArea.onTriggerExit    += OnLargeAreaExit;
            frontArea.onTriggerEnter   += OnFrontAreaEnter;
            frontArea.onTriggerExit    += OnFrontAreaExit;
            passingArea.onTriggerEnter += OnPassingAreaEnter;
            passingArea.onTriggerExit  += OnPassingAreaExit;
        }

        private void OnLargeAreaEnter(Collider other)
        {
            if(!frontArea.Contains(other))
                OnOutsideToLarge?.Invoke(other);
        }

        private void OnLargeAreaExit(Collider other)
        {
            if(!frontArea.collidersInsideLastFrame.Contains(other))
                OnLargeToOutside?.Invoke(other);
        } 

        private void OnFrontAreaEnter(Collider other)
        {
            // Entered from passing area
            if (passingArea.Contains(other) || passingArea.collidersInsideLastFrame.Contains(other))
                return;

            // Entered from outside
            OnLargeToFront?.Invoke(other);
        }
        
        private void OnFrontAreaExit(Collider other)
        {
            // Entered to passing area
            if(passingArea.collidersInsideLastFrame.Contains(other))
                return;

            // Entered to outside
            OnFrontToLarge?.Invoke(other);
        }
        
        private void OnPassingAreaEnter(Collider other)
        {
            // Entered from front area
            if(frontArea.collidersInsideLastFrame.Contains(other))
                OnFrontToPass?.Invoke(other);
            
            // Entered from inside area
            else if(insideArea.collidersInsideLastFrame.Contains(other))
                OnInsideToPass?.Invoke(other);
            
            else
                Debugger.LogWarning($"Collider {other} entered passing area from unknown area");
        }
        
        private void OnPassingAreaExit(Collider other)
        {
            // Entered to front area
            if (frontArea.Contains(other))
                OnPassToFront?.Invoke(other);
            
            // Entered to back area
            else if (insideArea.Contains(other))
                OnPassToInside?.Invoke(other);
            
            else
                Debugger.LogWarning($"Collider {other} exited passing area to unknown area");
        }
    }
}