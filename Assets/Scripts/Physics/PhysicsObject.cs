using System;
using System.Collections.Generic;
using PerceptionVR.Extensions;
using PerceptionVR.Common;
using PerceptionVR.Debug;
using UnityEngine;

namespace PerceptionVR.Physics
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsObject: MonoBehaviourBase
    {
        [SerializeField] private Direction startingGravityDirection = Direction.Down;

        [SerializeField] private bool insomnia;
        
        private readonly List<PhysicsObject> touchingObjects = new();
        
        public event Action OnSleep;
        public event Action OnAwake;

        public new Rigidbody rigidbody { get; private set; }
        public Quaternion gravityRotation { get; set; } = Quaternion.identity;

        private MeshFilter meshFilter;

        [SerializeField] private float averageKineticEnergy;
        [SerializeField] private float averageVelocity;
        [SerializeField] private float sleepTimer;
        [SerializeField] private float averagePositionDelta;
        private Vector3 previousPosition;

        private bool sleptLastFrame;

        private void Awake()
        {
            this.rigidbody = GetComponent<Rigidbody>();
            this.meshFilter = GetComponent<MeshFilter>();
            this.gravityRotation = Quaternion.FromToRotation(Vector3.down, startingGravityDirection.ToVector3());
            previousPosition = transform.position;
        }
        
        private void Start() => sleepTimer = PhysicsSettings.timeToSleep;

        private void FixedUpdate()
        {
            var isSleeping = rigidbody.IsSleeping();
            
            // RB is awake
            if (!isSleeping)
            {
                averagePositionDelta = (averagePositionDelta * (PhysicsSettings.nFrames - 1) + (transform.position - previousPosition).magnitude / Time.fixedDeltaTime) / PhysicsSettings.nFrames;
                
                // RB woke up by physics engine
                if (sleptLastFrame)
                {
                    Debugger.LogInfo($"{this} woke up by physics engine");
                    OnAwake?.Invoke();
                    sleepTimer = PhysicsSettings.timeToSleep;
                    if(PhysicsSettings.debugSleep && meshFilter != null)
                        meshFilter.mesh.SetVertexColor(Color.white);
                }

                sleepTimer = !insomnia && averagePositionDelta < PhysicsSettings.sleepThreshold ? sleepTimer - Time.fixedDeltaTime : PhysicsSettings.timeToSleep;

                // RB is going to sleep
                if (sleepTimer <= 0)
                    Sleep();

                // Gravity
                else
                    rigidbody.AddForce(PhysicsSettings.gravityStrength * (gravityRotation * Vector3.down), ForceMode.Acceleration);

                previousPosition = transform.position;
            }
            
            sleptLastFrame = isSleeping;
        }

        public void Sleep()
        {
            if (averagePositionDelta < PhysicsSettings.sleepThreshold && !rigidbody.IsSleeping())
            {
                rigidbody.Sleep();
                OnSleep?.Invoke();
                if(PhysicsSettings.debugSleep && meshFilter != null)
                    meshFilter.mesh.SetVertexColor(Color.blue);
            }
        }
        
        public void WakeUp()
        {
            if(rigidbody.IsSleeping())
            {
                sleptLastFrame = false;
                sleepTimer = PhysicsSettings.timeToSleep;
                rigidbody.WakeUp();
                OnAwake?.Invoke();
                if(PhysicsSettings.debugSleep && meshFilter != null)
                    meshFilter.mesh.SetVertexColor(Color.white);
            }
        }

        
        // Island system
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.rigidbody != null && collision.rigidbody.TryGetComponent<PhysicsObject>(out var touchedPhysicObject))
            {
                touchedPhysicObject.OnSleep += Sleep;
                touchedPhysicObject.OnAwake += WakeUp;
                touchingObjects.Add(touchedPhysicObject);
            }
        }

        public void OnCollisionExit(Collision collision)
        {
            if (collision.rigidbody != null && collision.rigidbody.TryGetComponent<PhysicsObject>(out var untouchedPhysicObject))
            {
                untouchedPhysicObject.OnSleep -= Sleep;
                untouchedPhysicObject.OnAwake -= WakeUp;
                touchingObjects.Remove(untouchedPhysicObject);
            }
        }

        private void OnDestroy()
        {
            foreach (var touchingObject in touchingObjects)
            {
                touchingObject.OnSleep -= Sleep;
                touchingObject.OnAwake -= WakeUp;
            }   
        }
    }
}

