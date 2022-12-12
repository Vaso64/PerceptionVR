using System;
using PerceptionVR.Debug;
using UnityEngine;

namespace PerceptionVR.Puzzle
{
    // TODO Gravity independent
    [RequireComponent(typeof(ConfigurableJoint))]
    public class Button : BoolControlBase
    {
        [SerializeField] private Transform button;
        
        public float buttonHeight = 0.1f;
        public float buttonOffset = 0.05f;
        public float pressure = 10f;
        
        private bool _wasPressedLastFrame;

        public void Awake()
        {
            var joint = GetComponent<ConfigurableJoint>();
            joint.anchor = new Vector3(0, buttonHeight, 0);
            joint.linearLimit = new SoftJointLimit { limit = buttonOffset };
            joint.yDrive = new JointDrive { positionSpring = pressure, positionDamper = 1f, maximumForce = Mathf.Infinity };
            
            OnActivated.AddListener(() => Debugger.LogInfo("activated"));
            OnDeactivated.AddListener(() => Debugger.LogInfo("deactivated"));
        }
        
        public void Update()
        {
            var isPressed = button.localPosition.y < buttonHeight;
            if (!_wasPressedLastFrame && isPressed)
            {
                // Deactivate
                if (isActivated)
                {
                    isActivated = false;
                    OnDeactivated.Invoke();
                    OnChanged.Invoke(false);
                }
                
                // Activate
                else
                {
                    isActivated = true;
                    OnActivated.Invoke();
                    OnChanged.Invoke(true);
                }
            }
            _wasPressedLastFrame = isPressed;
        }
    }
}