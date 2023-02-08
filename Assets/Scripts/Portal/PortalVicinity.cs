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
        [SerializeField] public SubscribableSwapTrigger insideArea;
        
        private List<Collider> frontAreaLastFrame;
        private List<Collider> passingAreaLastFrame;
        private List<Collider> insideAreaLastFrame;

        public event Action<Collider> OnOutsideToFront;
        public event Action<Collider> OnFrontToOutside;
        public event Action<Collider> OnFrontToPass;
        public event Action<Collider> OnPassToFront;
        public event Action<Collider> OnPassToInside;
        public event Action<Collider> OnInsideToPass;
        public       Action<GameObject> OnCloneIn; // Invoked by PortalCloneSystem


        private void Awake()
        {
            frontArea.onTriggerEnter   += OnFrontAreaEnter;
            frontArea.onTriggerExit    += OnFrontAreaExit;
            passingArea.onTriggerEnter += OnPassingAreaEnter;
            passingArea.onTriggerExit  += OnPassingAreaExit;
        }

        private void FixedUpdate()
        {
            insideAreaLastFrame  = insideArea.ToList();
            passingAreaLastFrame = passingArea.ToList();
            frontAreaLastFrame   = frontArea.ToList();
        }

        private void OnFrontAreaEnter(Collider other)
        {
            // Entered from passing area
            if (passingArea.Contains(other) || passingAreaLastFrame.Contains(other))
                return;

            // Entered from outside
            else
                OnOutsideToFront?.Invoke(other);
        }
        
        private void OnFrontAreaExit(Collider other)
        {
            // Entered to passing area
            if(passingArea.Contains(other))
                return;

            // Entered to outside
            else
                OnFrontToOutside?.Invoke(other);
        }
        
        private void OnPassingAreaEnter(Collider other)
        {
            // Entered from front area
            if(frontAreaLastFrame.Contains(other))
                OnFrontToPass?.Invoke(other);
            
            // Entered from inside area
            else if(insideAreaLastFrame.Contains(other))
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