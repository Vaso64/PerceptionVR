using UnityEngine;

namespace PerceptionVR.Player
{
    [RequireComponent(typeof (Camera))]
    public class PlayerCamera : MonoBehaviour
    {
        public static event Camera.CameraCallback OnBeforePlayerCameraRender;

        private Camera playerCamera;

        private void Start() => playerCamera = GetComponent<Camera>();

        private void OnPreRender() => OnBeforePlayerCameraRender?.Invoke(this.playerCamera);
    }
}