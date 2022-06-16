using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PerceptionVR.Portal;

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

        private Quaternion worldRotation;

        private PlayerInputAction.KBMPlayerActions playerActions;

        private void Awake()
        {
            this.worldRotation = Quaternion.identity;
            var playerInputAction = new PlayerInputAction();
            playerInputAction.Enable();
            this.playerActions = playerInputAction.KBMPlayer;
            PortalBase.OnTeleport.Add(transform, OnTeleport);
        }

        private void OnTeleport(Quaternion portalDelta) => worldRotation = portalDelta * worldRotation;
        

        protected virtual void Update()
        {
            // Controls
            if(!playerActions.ControlBlock.IsPressed())
            {                                                 // Sprint
                Move(playerActions.Move.ReadValue<Vector3>(), playerActions.Sprint.IsPressed());
                Look(playerActions.Look.ReadValue<Vector2>());
            }

            Debug.DrawRay(transform.position, this.worldRotation * Vector3.up, Color.red);
        }

        public void Move(Vector3 direction, bool sprint)
        {
            direction *= Time.deltaTime * moveSpeed * (sprint ? sprintMultiplier : 1);

            // Forward & Backward
            transform.position += transform.forward * direction.z;

            // Left & Right
            transform.position += this.worldRotation * Vector3.up * direction.y;

            // Up & Down
            transform.position += transform.right * direction.x;
        }


        public void Look(Vector2 direction)
        {
            direction *= lookSpeed;

            // Pitch
            transform.RotateAround(transform.position, this.worldRotation * Vector3.up, direction.x);

            // Yaw      
            transform.RotateAround(transform.position, transform.right, direction.y * -1);
        }
    }
}
