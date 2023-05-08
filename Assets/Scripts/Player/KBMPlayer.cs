using PerceptionVR.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PerceptionVR.Player
{
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
        
        private Grabbable grabbable;
        private Grabbable holdingItem;
        private TeleportableJoint grabJoint;
        private Ray grabRay;


        private PlayerInputAction.KBMPlayerActions playerActions;

        private void OnValidate()
        {
            var joint = GetComponent<ConfigurableJoint>();
            joint.xDrive = new JointDrive {positionSpring = grabSpring, positionDamper = grabDamper};
            joint.yDrive = new JointDrive {positionSpring = grabSpring, positionDamper = grabDamper};
            joint.zDrive = new JointDrive {positionSpring = grabSpring, positionDamper = grabDamper};
        }

        private void Start()
        {
            var playerInputAction = new PlayerInputAction();
            playerInputAction.Enable();
            this.playerActions = playerInputAction.KBMPlayer;
            grabJoint = GetComponent<TeleportableJoint>();
            this.playerActions.Grab.started += _ => OnGrab();
        }

        protected virtual void FixedUpdate()
        {
            InputSystem.Update();

            // Movement
            if(!playerActions.ControlBlock.IsPressed())
            {
                Move(playerActions.Move.ReadValue<Vector3>() * transform.lossyScale.x, playerActions.Sprint.IsPressed());
                Look(playerActions.Look.ReadValue<Vector2>());
            }
            
            // Grab detection
            grabRay.origin = transform.position;
            grabRay.direction = transform.forward;
            grabbable = UnityEngine.Physics.Raycast(grabRay, out var hit, grabRange, UnityEngine.Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) 
                            ? hit.collider.GetComponent<Grabbable>() 
                            : null;
        }
    
        private void Move(Vector3 direction, bool sprint)
        {
            direction *= Time.deltaTime * moveSpeed * (sprint ? sprintMultiplier : 1);

            // Forward & Backward
            transform.position += transform.forward * direction.z;

            // Up & Down
            transform.position += playerPhysicsObject.gravityRotation * Vector3.up * direction.y;

            // Left & Right
            transform.position += transform.right * direction.x;
        }


        private void Look(Vector2 direction)
        {
            direction *= lookSpeed;

            // Pitch
            transform.RotateAround(transform.position, playerPhysicsObject.gravityRotation * Vector3.up, direction.x);

            // Yaw      
            transform.RotateAround(transform.position, transform.right, direction.y * -1);
        }

        private void OnGrab()
        {
            // Release
            if (grabJoint.isConnectedToBody)
            {
                holdingItem.physicsObject.insomnia = false;
                grabJoint.ReleaseConnectedBody();
                holdingItem = null;
            }
                
            // Grab
            else if (grabbable != null)
            {
                holdingItem = grabbable;
                holdingItem.physicsObject.insomnia = true;
                grabJoint.SetConnectedBody(grabbable.physicsObject.rigidbody);
            }
                
        }
    }
}
