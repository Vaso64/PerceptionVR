using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PerceptionVR.Extensions;
using UnityEngine.ProBuilder;
using MoreLinq.Extensions;
using PerceptionVR.Global;
using PerceptionVR.Portal;
using PerceptionVR.Props;


namespace PerceptionVR.Player
{
    public class VRPlayerHand : MonoBehaviour, ISubTeleportable
    {
        private VRPlayerInput playerInput;
        
        [SerializeField] private VRPlayerHandSide handSide;

        private List<IGrabbable> grabbableItems = new List<IGrabbable>();
        
        private IGrabbable holdingItem = null;

        private FixedJoint grabJoint;

        public void OnCreateClone(GameObject clone)
        {
            clone.GetComponent<Renderer>().material.color = Color.gray;
        }

        public void TransferBehaviour(ISubTeleportable from, ISubTeleportable to)
        {
            from.transform.GetComponent<Renderer>().material.color = Color.gray;
            to.transform.GetComponent<Renderer>().material.color = Color.red;
        }

        private void Awake()
        {
            GetComponent<Renderer>().material.color = Color.red;
            
            
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
            if (grabbable == null || grabbableItems.Contains(grabbable))
                return;
            
            grabbableItems.Add(grabbable);
            Debug.Log(other.transform.name + "(grabbable) entered " + handSide + " hand vicinity");
        }
        
        private void OnTriggerExit(Collider other)
        {
            var grabbable = other.GetComponent<IGrabbable>();
            if (grabbable == null)
                return;
            
            grabbableItems.Remove(grabbable);
            Debug.Log(other.transform.name + "(grabbable) exited " + handSide + " hand vicinity");
        }

        private void OnGrab()
        {
            if(grabbableItems.Count == 0)
                return;
            
            // Get nearest grabbable item
            holdingItem = grabbableItems.MinBy(item => Vector3.Distance(item.collider.ClosestPoint(transform.position), transform.position)).First();

            // Create fixed grab joint to hold item
            Debug.Log(holdingItem.transform.name + " grabbed by " + handSide + " hand");
            grabJoint = gameObject.AddComponentNotify<FixedJoint>();
            grabJoint.connectedBody = holdingItem.rigidbody;
            
        }
        
        private void OnRelease()
        {
            if (holdingItem == null)
                return;

            // Destroy grab joint
            Debug.Log(holdingItem.transform.name + " released by " + handSide + " hand");
            gameObject.RemoveComponentNotify(grabJoint);
            holdingItem = null;
        }
    }
}
