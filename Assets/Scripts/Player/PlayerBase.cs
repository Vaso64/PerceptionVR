using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerceptionVR.Player
{
    public class PlayerBase : MonoBehaviour
    {

        [SerializeField]
        private Camera playerCamera;

        // Start is called before the first frame update
        protected virtual void Start()
        {       
            // Prevents the camera from clipping through the portal
            playerCamera.nearClipPlane = 0.00001f;
        }
    }
}
