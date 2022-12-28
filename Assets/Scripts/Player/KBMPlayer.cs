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
        

        private PlayerInputAction.KBMPlayerActions playerActions;

        protected override void Awake()
        {
            base.Awake();
            var playerInputAction = new PlayerInputAction();
            playerInputAction.Enable();
            this.playerActions = playerInputAction.KBMPlayer;
        }

        protected virtual void FixedUpdate()
        {
            InputSystem.Update();

            // Controls
            if(!playerActions.ControlBlock.IsPressed())
            {
                Move(playerActions.Move.ReadValue<Vector3>(), playerActions.Sprint.IsPressed());
                Look(playerActions.Look.ReadValue<Vector2>());
            }
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
    }
}
