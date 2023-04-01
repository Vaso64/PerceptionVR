using System;
using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Physics;

namespace PerceptionVR.Portals
{
    [RequireComponent(typeof(PhysicsObject))]
    public class PhysicsObjectClone : CloneBase<PhysicsObject>
    {
        private PhysicsObject physicsObject;

        private void Awake() => physicsObject = GetComponent<PhysicsObject>();


        public override void Track(PhysicsObject target, Portal throughPortal)
        {
            // Unregister from old target
            if(targetSet)
            {
                currentTarget.OnAwake -= physicsObject.WakeUp;
                currentTarget.OnSleep -= physicsObject.Sleep;
            }
            
            // Register to new
            target.OnAwake += physicsObject.WakeUp;
            target.OnSleep += physicsObject.Sleep;
            
            // Position object
            physicsObject.transform.SetPose(throughPortal.PairPose(target.transform.GetPose(), out var rotationDelta));
            
            // Set gravity
            physicsObject.rigidbody.useGravity = target.rigidbody.useGravity;
            physicsObject.gravityRotation = rotationDelta * target.gravityRotation;
            
            base.Track(target, throughPortal);
        }
        
        private void FixedUpdate()
        {
            if (targetSet && !physicsObject.rigidbody.IsSleeping())
            {
                var pairPose = currentThroughPortal.PairPose(currentTarget.transform.GetPose(), out var rotationDelta);
                
                UnityEngine.Debug.DrawLine(transform.position, pairPose.position, Color.red);
                
                // Move by RBMove
                physicsObject.rigidbody.MovePosition(pairPose.position);
                physicsObject.rigidbody.MoveRotation(pairPose.rotation);
                physicsObject.rigidbody.velocity = rotationDelta * currentTarget.rigidbody.velocity;
                
                // Move by velocity
                //physicsObject.rigidbody.VelocityPosition(pairPose);
                
                // Move by transform
                //physicsObject.transform.SetPose(pairPose);
                
                // Move by RB pose
                //physicsObject.rigidbody.position = pairPose.position;
                //physicsObject.rigidbody.rotation = pairPose.rotation;
            }
                
        }

        private void OnDestroy()
        {
            // Unregister from target
            if(targetSet)
            {
                currentTarget.OnAwake -= physicsObject.WakeUp;
                currentTarget.OnSleep -= physicsObject.Sleep;
            }
        }
    }
}