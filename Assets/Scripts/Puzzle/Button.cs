using PerceptionVR.Extensions;
using PerceptionVR.Physics;
using UnityEngine;
using UnityEngine.Events;

namespace PerceptionVR.Puzzle
{
    // TODO Gravity independent
    [RequireComponent(typeof(ConfigurableJoint))]
    public class Button : MonoBehaviourBase
    {
        public UnityEvent OnPressed;
        public UnityEvent OnReleased;
        public UnityEvent<bool> OnChanged;
        public bool isPressed { get; private set; }

        [SerializeField] private PhysicsObject button;
        [SerializeField] private float buttonHeight = 0.3f;
        [SerializeField] private float buttonOffset = 0.15f;
        [SerializeField] private float pressure = 10f;
        
        private MeshFilter meshFilter;

        private void Awake()
        {
            meshFilter = button.GetComponentInChildren<MeshFilter>();
            isPressed = false;
            
            // Setup joint
            button.gravityRotation = transform.rotation;
            button.transform.localPosition = new Vector3(0, buttonHeight + buttonOffset, 0);
            var joint = GetComponent<ConfigurableJoint>();
            joint.anchor = new Vector3(0, buttonHeight, 0);
            joint.linearLimit = new SoftJointLimit { limit = buttonOffset };
            joint.yDrive = new JointDrive { positionSpring = pressure, positionDamper = 1f, maximumForce = Mathf.Infinity };
        }
        
        private void Update()
        {
            // Button pressed this frame
            if (!isPressed && button.transform.localPosition.y < buttonHeight)
            {
                isPressed = true;
                OnPressed?.Invoke();
                OnChanged?.Invoke(true);
            }   
                
            // Button released this frame
            if (isPressed && button.transform.localPosition.y > buttonHeight)
            {
                isPressed = false;
                OnReleased?.Invoke();
                OnChanged?.Invoke(false);
            }
        }
        
        public void SetColor(Color color) => meshFilter.mesh.SetVertexColor(color);
    }
}