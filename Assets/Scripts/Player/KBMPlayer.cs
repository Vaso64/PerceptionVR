using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PerceptionVR.Player
{
    public class KBMPlayer : PlayerBase
    {
        [Range(0, 10)]
        public float moveSpeed;

        [Range(0, 3)]
        public float moveSpeedMultiplier;

        [Range(0, 1)]
        public float lookSpeed;

        private PlayerInputAction.PlayerActions playerActions;

        private void Awake()
        {
            var playerInputAction = new PlayerInputAction();
            playerInputAction.Enable();
            this.playerActions = playerInputAction.Player;
        }

        protected override void Update()
        {
            // Controls
            if(!playerActions.ControlBlock.IsPressed())
            {                                                 // Sprint
                Move(playerActions.Move.ReadValue<Vector3>(), playerActions.Sprint.IsPressed());
                Look(playerActions.Look.ReadValue<Vector2>());
            }

            base.Update();
        }

        public void Move(Vector3 direction, bool sprint)
        {
            direction *= Time.deltaTime * moveSpeed * (sprint ? moveSpeedMultiplier : 1);

            // Forward & Backward
            transform.position += transform.forward * direction.z;

            // Left & Right
            transform.position += Vector3.up * direction.y;

            // Up & Down
            transform.position += transform.right * direction.x;
        }


        public void Look(Vector2 direction)
        {
            direction *= lookSpeed;

            // Pitch
            transform.RotateAround(transform.position, Vector3.up, direction.x);

            // Yaw      
            transform.RotateAround(transform.position, transform.right, direction.y * -1);
        }
    }
}
