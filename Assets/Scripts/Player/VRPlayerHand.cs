using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Common;
using MoreLinq.Extensions;
using PerceptionVR.Portals;
using PerceptionVR.Debug;
using PerceptionVR.Physics;


namespace PerceptionVR.Player
{
    //[RequireComponent(typeof(TeleportableJoint))]
    // TODO: Teleportable behaviour
    public class VRPlayerHand: MonoBehaviourBase
    {
        private VRPlayerInput playerInput;
        
        [SerializeField] private Side handSide;
        
        [SerializeField] private TeleportableJoint bodyJoint;

        // Grabbing
        [SerializeField] private TeleportableJoint grabJoint;
        [SerializeField] private float grabSpring = 1000;
        [SerializeField] private float grabDamper = 10;
        [SerializeField] private float grabAngularSpring = 360;
        [SerializeField] private float grabAngularDamper = 1;
        private Grabbable holdingItem;
        private List<Grabbable> grabbableItems = new();


        private void Awake()
        {
            GetComponentInChildren<Renderer>().material.color = Color.red;
        }

        private void Start()
        {
            this.playerInput = FindObjectOfType<VRPlayerInput>();
            
            // Register hand to player input events
            switch (handSide)
            {
                case Side.Left:
                    playerInput.OnLeftControllerGrabbed += OnGrab;
                    playerInput.OnLeftControllerReleased += OnRelease;
                    break;
                case Side.Right:
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
                case Side.Left:
                    controllerPose = playerInput.leftControllerPose;
                    break;
                case Side.Right:
                    controllerPose = playerInput.rightControllerPose;
                    break;
                default:
                    controllerPose = new Pose();
                    break;
            }
            
            // Move hand
            bodyJoint.joint.targetPosition = transform.lossyScale.x * (controllerPose.position - playerInput.hmdPose.position.x0z());
            if(!controllerPose.rotation.IsNaN())
                bodyJoint.joint.targetRotation = controllerPose.rotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            var grabbable = other.GetComponent<Grabbable>();
            if (grabbable == null || grabbableItems.Contains(grabbable))
                return;
            
            grabbableItems.Add(grabbable);
            Debugger.LogInfo(other.transform.name + "(grabbable) entered " + handSide + " hand vicinity");
        }
        
        private void OnTriggerExit(Collider other)
        {
            var grabbable = other.GetComponent<Grabbable>();
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
            holdingItem.physicsObject.insomnia = true;
            grabJoint.SetConnectedBody(holdingItem.physicsObject.rigidbody);
            grabJoint.joint.SetDrive(grabSpring, grabDamper);
            grabJoint.joint.SetAngularDrive(grabAngularSpring, grabAngularDamper);
        }
        
        private void OnRelease()
        {
            if (holdingItem == null)
                return;

            // Destroy grab joint
            Debugger.LogInfo(holdingItem.transform.name + " released by " + handSide + " hand");
            holdingItem.physicsObject.insomnia = false;
            grabJoint.ReleaseConnectedBody();
            grabJoint.joint.SetDrive(0, 0);
            grabJoint.joint.SetAngularDrive(0, 0);
            holdingItem = null;
        }
    }
}
