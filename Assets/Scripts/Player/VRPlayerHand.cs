using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Common;
using MoreLinq.Extensions;
using PerceptionVR.Portal;
using PerceptionVR.Debug;


namespace PerceptionVR.Player
{
    //[RequireComponent(typeof(ConfigurableJoint))]
    public class VRPlayerHand : MonoBehaviour, ITeleportableBehaviour
    {
        public enum VRPlayerHandSide
        {
            Left,
            Right
        }
        
        private VRPlayerInput playerInput;
        
        [SerializeField] private VRPlayerHandSide handSide;
        
        private ConfigurableJoint bodyJoint;
        
        // Grabbing
        private List<IGrabbable> grabbableItems = new();
        private IGrabbable holdingItem = null;
        private FixedJoint grabJoint;

        public void OnCreateClone(GameObject clone, out IEnumerable<Type> preservedComponents)
        {
            preservedComponents = Enumerable.Empty<Type>();
            clone.GetComponent<Renderer>().material.color = Color.gray;
        }

        public void TransferBehaviour(GameObject from, GameObject to)
        {
            // TODO: Transfer behaviour
            from.GetComponent<Renderer>().material.color = Color.gray;
            to.GetComponent<Renderer>().material.color = Color.red;
        }

        private void Awake()
        {
            GetComponent<Renderer>().material.color = Color.red;
        }

        private void Start()
        {
            this.bodyJoint = GetComponent<ConfigurableJoint>();
            
            this.playerInput = GetComponentInParent<VRPlayerInput>();
            
            // Register hand to player input events
            switch (handSide)
            {
                case VRPlayerHandSide.Left:
                    playerInput.OnLeftControllerGrabbed += OnGrab;
                    playerInput.OnLeftControllerReleased += OnRelease;
                    break;
                case VRPlayerHandSide.Right:
                    playerInput.OnRightControllerGrabbed += OnGrab;
                    playerInput.OnRightControllerReleased += OnRelease;
                    break;
            }
        }


        private void FixedUpdate()
        {
            // Get controller pose
            Pose controllerPose;
            switch (handSide)
            {
                case VRPlayerHandSide.Left:
                    controllerPose = playerInput.leftControllerPose;
                    break;
                case VRPlayerHandSide.Right:
                    controllerPose = playerInput.rightControllerPose;
                    break;
                default:
                    controllerPose = new Pose();
                    break;
            }
            
            // Move hand
            bodyJoint.targetPosition = controllerPose.position - playerInput.hmdPose.position.x0z();
            if(!controllerPose.rotation.IsNaN())
                bodyJoint.targetRotation = controllerPose.rotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            var grabbable = other.GetComponent<IGrabbable>();
            if (grabbable == null || grabbableItems.Contains(grabbable))
                return;
            
            grabbableItems.Add(grabbable);
            Debugger.LogInfo(other.transform.name + "(grabbable) entered " + handSide + " hand vicinity");
        }
        
        private void OnTriggerExit(Collider other)
        {
            var grabbable = other.GetComponent<IGrabbable>();
            if (grabbable == null)
                return;
            
            grabbableItems.Remove(grabbable);
            Debugger.LogInfo(other.transform.name + "(grabbable) exited " + handSide + " hand vicinity");
        }

        private void OnGrab()
        {
            if(grabbableItems.Count == 0)
                return;
            
            // Get nearest grabbable item
            holdingItem = grabbableItems.MinBy(item => Vector3.Distance(item.collider.ClosestPoint(transform.position), transform.position)).First();

            // Create fixed grab joint to hold item
            Debugger.LogInfo(holdingItem.transform.name + " grabbed by " + handSide + " hand");
            grabJoint = gameObject.AddComponentNotify<FixedJoint>();
            grabJoint.connectedBody = holdingItem.rigidbody;
        }
        
        private void OnRelease()
        {
            if (holdingItem == null)
                return;

            // Destroy grab joint
            Debugger.LogInfo(holdingItem.transform.name + " released by " + handSide + " hand");
            gameObject.RemoveComponentNotify(grabJoint);
            holdingItem = null;
        }
    }
}
