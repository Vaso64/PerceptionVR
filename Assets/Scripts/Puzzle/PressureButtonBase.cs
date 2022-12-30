using PerceptionVR.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace PerceptionVR.Puzzle
{
    // TODO Gravity independent
    [RequireComponent(typeof(ConfigurableJoint))]
    public abstract class PressureButtonBase : ControlBase
    {
        [SerializeField] private Transform button;
        
        public float buttonHeight = 0.3f;
        public float buttonOffset = 0.15f;
        public float pressure = 10f;
        
        protected bool wasPressedLastFrame;
        protected bool isPressedThisFrame;

        private void Awake()
        {
            // Setup joint
            var joint = GetComponent<ConfigurableJoint>();
            joint.anchor = new Vector3(0, buttonHeight, 0);
            joint.linearLimit = new SoftJointLimit { limit = buttonOffset };
            joint.yDrive = new JointDrive { positionSpring = pressure, positionDamper = 1f, maximumForce = Mathf.Infinity };
            
            OnChanged.AddListener(SetButtonColor);
        }
        
        protected virtual void Update()
        {
            wasPressedLastFrame = isPressedThisFrame;
            isPressedThisFrame = button.localPosition.y < buttonHeight;
        }
        
        private void SetButtonColor(bool active) => button.GetComponent<MeshFilter>().mesh.SetVertexColor(active ? Color.green : Color.red);
    }
}