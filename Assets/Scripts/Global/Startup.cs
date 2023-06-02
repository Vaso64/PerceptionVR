using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PerceptionVR.Common;
using PerceptionVR.Debug;
using UnityEngine.XR.Management;

namespace PerceptionVR.Global
{
    public class Startup: MonoBehaviourBase
    {
        [SerializeField] private bool manualArgsEnabled;
        [SerializeField] private string[] manualArgs;
        
        [SerializeField] private List<GameObject> vrOnlyObjects;
        [SerializeField] private List<Behaviour>  vrOnlyComponents;
        [SerializeField] private List<GameObject> kbmOnlyObjects;
        [SerializeField] private List<Behaviour>  kbmOnlyComponents;
        [SerializeField] private GameObject       spectatorCamera;
        
        
        private void Start()
        {
            var args = manualArgsEnabled ? manualArgs : Environment.GetCommandLineArgs();
            
            Debugger.LogInfo(string.Join(", ", args));
            
            // Set mode (VR or KBM)
            var mode = args.Contains("-vr") ? DisplayMode.VR : DisplayMode.Desktop;
            SetMode(mode);
            
            // Enable spectate camera
            spectatorCamera.SetActive(mode == DisplayMode.VR && args.Contains("-spectate"));
        }
        
        private void SetMode(DisplayMode mode)
        {
            var vrEnabled = mode == DisplayMode.VR;
            
            vrOnlyComponents.ForEach(c =>  c.enabled = vrEnabled);
            kbmOnlyComponents.ForEach(c => c.enabled = !vrEnabled);
            vrOnlyObjects.ForEach(o =>  o.SetActive( vrEnabled));
            kbmOnlyObjects.ForEach(o => o.SetActive(!vrEnabled));
            
            // Start XR session if VR is enabled
            if(vrEnabled)
                StartCoroutine(StartXR());
        }

        private void OnDestroy()
        {
            if(XRGeneralSettings.Instance.Manager.activeLoader != null)
                StopXR();
        } 


        private static IEnumerator StartXR()
        {
            Debugger.LogInfo("Initializing XR...");
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debugger.LogError("Initializing XR Failed. Check Editor or Player log for details.");
            }
            else
            {
                Debugger.LogInfo("Starting XR...");
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
        }

        private static void StopXR()
        {
            Debugger.LogInfo("Stopping XR...");
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }
}