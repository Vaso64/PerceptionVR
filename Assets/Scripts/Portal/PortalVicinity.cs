using System;
using System.Collections.Generic;
using System.Linq;
using PerceptionVR.Debug;
using PerceptionVR.Physics;
using UnityEngine;

namespace PerceptionVR.Portals
{
    public class PortalVicinity : MonoBehaviour
    {
        [SerializeField] public SubscribableSwapTrigger frontArea;
        [SerializeField] public SubscribableSwapTrigger passingArea;
        [SerializeField] public SubscribableSwapTrigger backArea;
        
        private List<Collider> frontAreaLastFrame;
        private List<Collider> passingAreaLastFrame;
        private List<Collider> backAreaLastFrame;

        public event Action<Collider> OnOutsideToFront;
        public event Action<Collider> OnFrontToOutside;
        public event Action<Collider> OnFrontToPass;
        public event Action<Collider> OnPassToFront;
        public event Action<Collider> OnPassToBack;
        public event Action<Collider> OnBackToPass;
        public event Action<Collider> OnBackToOutside;
        public event Action<Collider> OnOutsideToBack;
        public event Action<Collider> OnFrontToBack;
        public event Action<Collider> OnBackToFront;


        private void Awake()
        {
            frontArea.onTriggerEnter   += OnFrontAreaEnter;
            frontArea.onTriggerExit    += OnFrontAreaExit;
            passingArea.onTriggerEnter += OnPassingAreaEnter;
            passingArea.onTriggerExit  += OnPassingAreaExit;
            backArea.onTriggerEnter    += OnBackAreaEnter;
            backArea.onTriggerExit     += OnBackAreaExit;
        }

        private void FixedUpdate()
        {
            backAreaLastFrame    = backArea.ToList();
            passingAreaLastFrame = passingArea.ToList();
            frontAreaLastFrame   = frontArea.ToList();
        }

        private void OnFrontAreaEnter(Collider other)
        {
            // Entered from passing area
            if (passingArea.Contains(other) || passingAreaLastFrame.Contains(other))
                return;

            // Entered from back area
            if(backAreaLastFrame.Contains(other))
                OnBackToFront?.Invoke(other);
            
            // Entered from outside
            else
                OnOutsideToFront?.Invoke(other);
        }
        
        private void OnFrontAreaExit(Collider other)
        {
            // Entered to passing area
            if(passingArea.Contains(other))
                return;
            
            // Entered to back area
            if(backArea.Contains(other))
                OnFrontToBack?.Invoke(other);
            
            // Entered to outside
            else
                OnFrontToOutside?.Invoke(other);
        }
        
        private void OnPassingAreaEnter(Collider other)
        {
            // Entered from front area
            if(frontAreaLastFrame.Contains(other))
                OnFrontToPass?.Invoke(other);
            
            // Entered from back area
            else if(backAreaLastFrame.Contains(other))
                OnBackToPass?.Invoke(other);
            
            else
                Debugger.LogWarning($"Collider {other} entered passing area from unknown area");
        }
        
        private void OnPassingAreaExit(Collider other)
        {
            // Entered to front area
            if (frontArea.Contains(other))
                OnPassToFront?.Invoke(other);
            
            // Entered to back area
            else if (backArea.Contains(other))
                OnPassToBack?.Invoke(other);
            
            else
                Debugger.LogWarning($"Collider {other} exited passing area to unknown area");
        }
        
        private void OnBackAreaEnter(Collider other)
        {
            // Entered from passing area
            if (passingArea.Contains(other) || frontArea.Contains(other) || passingAreaLastFrame.Contains(other) || frontAreaLastFrame.Contains(other))
                return;

            OnOutsideToBack?.Invoke(other);
        }
        
        private void OnBackAreaExit(Collider other)
        {
            if(passingArea.Contains(other) || frontArea.Contains(other))
                return;
            
            OnBackToOutside?.Invoke(other);
        }
    }
}