using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using System;
using System.Collections.Generic;
using PerceptionVR.Common;
using PerceptionVR.Debug;

namespace PerceptionVR.Global
{
    public class RenderingManagment: MonoBehaviourBase
    {
        public static int portalRenderCount;
        [SerializeField] private bool logPortalRenderCount = false;
        
        public static event Action<DisplayMode, Vector2Int> OnResolutionChange;
        public static readonly Dictionary<DisplayMode, Vector2Int> CurrentResolutions = new ()
        {
            { DisplayMode.Desktop, Vector2Int.zero },
            { DisplayMode.VR, Vector2Int.zero }
        };

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(ResolutionTracker());
        }

        private void Update()
        {
            if(logPortalRenderCount){
                Debugger.LogInfo($"Portal render count: {portalRenderCount}");
                portalRenderCount = 0;
            }
        }


        IEnumerator ResolutionTracker(){
            while(true){
                // VR
                if(XRSettings.enabled){
                    if(CurrentResolutions[DisplayMode.VR].x != XRSettings.eyeTextureWidth || CurrentResolutions[DisplayMode.VR].y != XRSettings.eyeTextureHeight){
                        Debugger.LogInfo($"Updating VR resolution to {XRSettings.eyeTextureWidth}x{XRSettings.eyeTextureHeight}");
                        CurrentResolutions[DisplayMode.VR] = new Vector2Int(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight);
                        OnResolutionChange?.Invoke(DisplayMode.VR, CurrentResolutions[DisplayMode.VR]);
                    }
                }

                // Flat
                if (CurrentResolutions[DisplayMode.Desktop].x != Screen.width || CurrentResolutions[DisplayMode.Desktop].y != Screen.height)
                {
                    Debugger.LogInfo($"Updating desktop resolution to {Screen.width}x{Screen.height}");
                    CurrentResolutions[DisplayMode.Desktop] = new Vector2Int(Screen.width, Screen.height);
                    OnResolutionChange?.Invoke(DisplayMode.Desktop, CurrentResolutions[DisplayMode.Desktop]);
                }

                // Check every second
                yield return new WaitForSeconds(1f);
            }
        }
    }
}


