using UnityEngine;

namespace PerceptionVR.Common
{
    public enum DisplayMode
    {
        Desktop,
        VR
    }
    
    public static class GameDisplayModeExtensions
    {
        public static DisplayMode GetDisplayMode(this Camera camera)
        {
            switch (camera.stereoTargetEye)
            {
                case StereoTargetEyeMask.Left:
                case StereoTargetEyeMask.Right:
                case StereoTargetEyeMask.Both:
                    return DisplayMode.VR;
                case StereoTargetEyeMask.None:
                default:
                    return DisplayMode.Desktop;
            }
        }
    }
    
    
}