using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PerceptionVR.Debug;
using PerceptionVR.Player;
using UnityEngine.XR.Management;

namespace PerceptionVR.Global
{
    public class Startup: MonoBehaviourBase
    {
        [SerializeField] private bool manualArgsEnabled;
        [SerializeField] private string[] manualArgs;
        
        private void Start()
        {
            var args = manualArgsEnabled ? manualArgs : Environment.GetCommandLineArgs();
            
            Debugger.LogInfo(string.Join(", ", args));
            
            // Set mode (VR or KBM)
            SetMode(args);
        }
        
        private void SetMode(IEnumerable<string> args)
        {
            if (args.Contains("-vr"))
            {
                FindObjectOfType<VRPlayer>(includeInactive: true).gameObject.SetActive(true);
                FindObjectOfType<KBMPlayer>(includeInactive: true).gameObject.SetActive(false);
                StartCoroutine(StartXR());
            }
            
            else
            {
                FindObjectOfType<VRPlayer>(includeInactive: true).gameObject.SetActive(false);
                FindObjectOfType<KBMPlayer>(includeInactive: true).gameObject.SetActive(true);
            }
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