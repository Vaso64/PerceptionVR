using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using System;
using System.Collections.Generic;
using PerceptionVR.Common;
using PerceptionVR.Debug;

namespace PerceptionVR.Global
{
    public class RenderingManagement: MonoBehaviourBase
    {
        // Resolution tracking
        public static event Action<DisplayMode, Vector2Int> OnResolutionChange;
        public static readonly Dictionary<DisplayMode, Vector2Int> CurrentResolutions = new ()
        {
            { DisplayMode.Desktop, Vector2Int.zero },
            { DisplayMode.VR, Vector2Int.zero }
        };

        // Portal rendering
        public const int PortalRecursionLimit = 2;
        private const int RenderTexturePoolSize = 16;
        private static readonly Dictionary<DisplayMode, Stack<RenderTexture>> RTPool = new()
        {
            { DisplayMode.VR, new Stack<RenderTexture>(RenderTexturePoolSize) },
            { DisplayMode.Desktop, new Stack<RenderTexture>(RenderTexturePoolSize) }
        };
        
        // Stats
        public static int portalRenderCount;
        private static int peakPoolUsageCount;
        [SerializeField] private bool logPortalRenderStats = false;


        private void Start()
        { 
            AllocateRTPool(DisplayMode.Desktop, new Vector2Int(Screen.width, Screen.height));
            AllocateRTPool(DisplayMode.VR, new Vector2Int(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight));
            StartCoroutine(ResolutionTracker());
        }

        private void Update()
        {
            if(logPortalRenderStats){
                Debugger.LogInfo($"Portal render count: {portalRenderCount}");
                portalRenderCount = 0;
                
                Debugger.LogInfo($"Highest pool usage count: {peakPoolUsageCount}");
                peakPoolUsageCount = 0;
            }
        }
        
        private static IEnumerator ResolutionTracker()
        {
            while(true){
                // VR
                if(XRSettings.enabled && (CurrentResolutions[DisplayMode.VR].x != XRSettings.eyeTextureWidth || CurrentResolutions[DisplayMode.VR].y != XRSettings.eyeTextureHeight))
                    UpdateResolution(DisplayMode.VR, new Vector2Int(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight));

                // Flat
                if (CurrentResolutions[DisplayMode.Desktop].x != Screen.width || CurrentResolutions[DisplayMode.Desktop].y != Screen.height)
                    UpdateResolution(DisplayMode.Desktop, new Vector2Int(Screen.width, Screen.height));

                // Check every second
                yield return new WaitForSeconds(1f);
            }
        }

        private static void UpdateResolution(DisplayMode displayMode, Vector2Int resolution)
        {
            Debugger.LogInfo($"Updating {displayMode} resolution to {resolution.x}x{resolution.y}");
            CurrentResolutions[displayMode] = resolution;
            AllocateRTPool(displayMode, resolution);
            OnResolutionChange?.Invoke(displayMode, resolution);
        }
        
        private static void AllocateRTPool(DisplayMode displayMode, Vector2Int resolution)
       {
           if (resolution == Vector2Int.zero) 
                return;
            
            // Release old RTs
            while (RTPool[displayMode].Count > 0)
                RTPool[displayMode].Pop().Release();
            
            // Allocate new RTs
            for (var i = 0; i < RenderTexturePoolSize; i++)
            {
                var rt = new RenderTexture(resolution.x, resolution.y, 24, RenderTextureFormat.Default)
                {
                    name = $"Portal RT {displayMode} #{i}",
                    antiAliasing = 1,
                    vrUsage = displayMode == DisplayMode.VR ? VRTextureUsage.OneEye : VRTextureUsage.None
                };
                //rt.Create();    Allocated on demand
                RTPool[displayMode].Push(rt);
            }
       }
        
        public static RenderTexture GetRTFromPool(DisplayMode displayMode)
        {
            var poolUsageCount = RenderTexturePoolSize - RTPool[displayMode].Count - 1;
            if (poolUsageCount > peakPoolUsageCount)
                peakPoolUsageCount = poolUsageCount;
            return RTPool[displayMode].Pop();
        }

        public static void ReturnRTToPool(DisplayMode displayMode, RenderTexture rt)
        { 
            rt.DiscardContents();
            RTPool[displayMode].Push(rt);
        }
        
        public static RenderTexture PeekRTPool(DisplayMode displayMode) => RTPool[displayMode].Peek();
    }
}


