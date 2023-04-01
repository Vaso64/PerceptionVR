using PerceptionVR.Common;
using PerceptionVR.Extensions;
using PerceptionVR.Physics;
using UnityEngine;
using UnityEngine.Serialization;

namespace PerceptionVR.Puzzle
{
    // TODO Gravity independent
    [RequireComponent(typeof(ConfigurableJoint))]
    public abstract class PressureButtonBase : ControlBase
    {
        [SerializeField] private PhysicsObject button;
        
        public float buttonHeight = 0.3f;
        public float buttonOffset = 0.15f;
        public float pressure = 10f;
        
        protected bool wasPressedLastFrame;
        protected bool isPressedThisFrame;

        private void Awake()
        {
            // Setup joint
            button.gravityRotation = transform.rotation;
            var joint = GetComponent<ConfigurableJoint>();
            joint.anchor = new Vector3(0, buttonHeight, 0);
            joint.linearLimit = new SoftJointLimit { limit = buttonOffset };
            joint.yDrive = new JointDrive { positionSpring = pressure, positionDamper = 1f, maximumForce = Mathf.Infinity };
            
            OnChanged.AddListener(SetButtonColor);
        }
        
        protected virtual void Update()
        {
            if(Time.time < 1f)
                return;
            
            wasPressedLastFrame = isPressedThisFrame;
            isPressedThisFrame = button.transform.localPosition.y < buttonHeight;
        }
        
        private void SetButtonColor(bool active) => button.GetComponent<MeshFilter>().mesh.SetVertexColor(active ? Color.green : Color.red);
    }
}