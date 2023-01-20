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
        public static DisplayMode ToGameDisplayMode(this StereoTargetEyeMask targetEye)
        {
            switch (targetEye)
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