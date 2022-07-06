using UnityEngine;

namespace PerceptionVR.Global
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


