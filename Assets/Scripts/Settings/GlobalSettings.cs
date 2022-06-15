using UnityEngine;

namespace PerceptionVR.Common
{
    public class GlobalSettings : MonoBehaviour
    {
        void Awake()
        {
            // Cap the framerate to screen refresh rate
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
        }
    }
}


