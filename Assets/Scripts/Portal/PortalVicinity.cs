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
        [SerializeField] public SubscribableSwapTrigger largeArea;
        [SerializeField] public SubscribableSwapTrigger frontArea;
        [SerializeField] public SubscribableSwapTrigger passingArea;
        [SerializeField] public SubscribableSwapTrigger insideArea;
        
        
        private readonly List<Collider> frontAreaLastFrame = new ();
        private readonly List<Collider> passingAreaLastFrame = new ();
        private readonly List<Collider> insideAreaLastFrame = new ();

        
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

        private void FixedUpdate()
        {
            insideAreaLastFrame.Clear();  insideAreaLastFrame.AddRange(insideArea);
            passingAreaLastFrame.Clear(); passingAreaLastFrame.AddRange(passingArea);
            frontAreaLastFrame.Clear();   frontAreaLastFrame.AddRange(frontArea);
        }

        private void OnLargeAreaEnter(Collider other)
        {
            if(!frontArea.Contains(other))
                OnOutsideToLarge?.Invoke(other);
        }

        private void OnLargeAreaExit(Collider other)
        {
            if(!frontAreaLastFrame.Contains(other))
                OnLargeToOutside?.Invoke(other);
        } 

        private void OnFrontAreaEnter(Collider other)
        {
            // Entered from passing area
            if (passingArea.Contains(other) || passingAreaLastFrame.Contains(other))
                return;

            // Entered from outside
            OnLargeToFront?.Invoke(other);
        }
        
        private void OnFrontAreaExit(Collider other)
        {
            // Entered to passing area
            if(passingAreaLastFrame.Contains(other))
                return;

            // Entered to outside
            OnFrontToLarge?.Invoke(other);
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