using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PerceptionVR.Extensions;
using UnityEngine.ProBuilder;
using MoreLinq.Extensions;
using PerceptionVR.Props;


namespace PerceptionVR.Player
{
    public class VRPlayerHand : MonoBehaviour
    {
        private VRPlayerInput playerInput;
        
        [SerializeField] private VRPlayerHandSide handSide;

        private List<IGrabbable> grabbableItems = new List<IGrabbable>();
        
        private IGrabbable holdingItem = null;
        
        

        private void Awake()
        {
            this.playerInput = GetComponentInParent<VRPlayerInput>();

            // Register hand to player input events
            switch (handSide)
            {
                case VRPlayerHandSide.Left:
                    playerInput.leftControllerGrabbed += OnGrab;
                    playerInput.leftControllerReleased += OnRelease;
                    break;
                case VRPlayerHandSide.Right:
                    playerInput.rightControllerGrabbed += OnGrab;
                    playerInput.rightControllerReleased += OnRelease;
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var grabbable = other.GetComponent<IGrabbable>();
            Debug.Log(other.transform.name + " entered " + handSide + " hand vicimity");
            if (grabbable != null)
            {
                grabbableItems.Add(grabbable);
                Debug.Log(other.transform.name + " is grabbable");
            }
                
        }
        
        private void OnTriggerExit(Collider other)
        {
            var grabbable = other.GetComponent<IGrabbable>();
            Debug.Log(other.transform.name + " exited " + handSide + " hand vicinity");
            if (grabbable != null)
                grabbableItems.Remove(grabbable);
        }

        private void OnGrab()
        {
            if(grabbableItems.Count == 0)
                return;
            
            // Get nearest grabbable item
            holdingItem = grabbableItems.MinBy(item => Vector3.Distance(item.collider.ClosestPoint(transform.position), transform.position)).First();
            var joint = holdingItem.transform.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = GetComponent<Rigidbody>();
            Debug.Log(holdingItem.transform.name + " grabbed by " + handSide + " hand");
        }
        
        private void OnRelease()
        {
            if (holdingItem == null)
                return;

            // Destory joint
            Destroy(holdingItem.transform.GetComponent<FixedJoint>());
            Debug.Log(holdingItem.transform.name + " released by " + handSide + " hand");
            holdingItem = null;
        }
    }
}
