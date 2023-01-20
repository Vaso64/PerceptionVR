using System;
using PerceptionVR.Common;
using PerceptionVR.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PerceptionVR.Player
{
    [RequireComponent(typeof(TeleportableJoint))]
    public class KBMPlayer : PlayerBase
    {
        [Range(0, 10)]
        public float moveSpeed;

        [Range(0, 3)]
        public float sprintMultiplier;

        [Range(0, 1)]
        public float lookSpeed;
        
        [Range(0,3)]
        public float grabRange;
        [SerializeField] private float grabSpring;
        [SerializeField] private float grabDamper;
        
        private IGrabbable grabbable;
        private TeleportableJoint grabJoint;



        private PlayerInputAction.KBMPlayerActions playerActions;

        protected override void Awake()
        {
            base.Awake();
            var playerInputAction = new PlayerInputAction();
            playerInputAction.Enable();
            this.playerActions = playerInputAction.KBMPlayer;
            grabJoint = GetComponent<TeleportableJoint>();
        }

        private void OnValidate()
        {
            var joint = GetComponent<ConfigurableJoint>();
            joint.xDrive = new JointDrive {positionSpring = grabSpring, positionDamper = grabDamper};
            joint.yDrive = new JointDrive {positionSpring = grabSpring, positionDamper = grabDamper};
            joint.zDrive = new JointDrive {positionSpring = grabSpring, positionDamper = grabDamper};
        }

        private void Start()
        {
            this.playerActions.Grab.started += _ => OnGrab();
        }

        protected virtual void FixedUpdate()
        {
            InputSystem.Update();

            // Movement
            if(!playerActions.ControlBlock.IsPressed())
            {
                Move(playerActions.Move.ReadValue<Vector3>(), playerActions.Sprint.IsPressed());
                Look(playerActions.Look.ReadValue<Vector2>());
            }
            
            // Grab detection
            grabbable = UnityEngine.Physics.Raycast(transform.position, transform.forward, out var hit, grabRange) ? hit.collider.GetComponent<IGrabbable>() : null;
        }
    
        private void Move(Vector3 direction, bool sprint)
        {
            direction *= Time.deltaTime * moveSpeed * (sprint ? sprintMultiplier : 1);

            // Forward & Backward
            transform.position += transform.forward * direction.z;

            // Up & Down
            transform.position += gravityDirection * Vector3.up * direction.y;

            // Left & Right
            transform.position += transform.right * direction.x;
        }


        private void Look(Vector2 direction)
        {
            direction *= lookSpeed;

            // Pitch
            transform.RotateAround(transform.position, gravityDirection * Vector3.up, direction.x);

            // Yaw      
            transform.RotateAround(transform.position, transform.right, direction.y * -1);
        }

        private void OnGrab()
        {
            if (grabJoint.isConnectedToBody)
                grabJoint.ReleaseConnectedBody();

            else if (grabbable != null)
                grabJoint.SetConnectedBody<FixedJoint>(grabbable.rigidbody);
        }
    }
}
