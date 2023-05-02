using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PerceptionVR.Debug;
using UnityEngine.XR.Management;

namespace PerceptionVR.Global
{
    public class Startup: MonoBehaviourBase
    {
        [SerializeField] private bool manualArgsEnabled;
        [SerializeField] private string[] manualArgs;
        
        [SerializeField] private List<GameObject> vrOnlyObjects;
        [SerializeField] private List<GameObject> kbmOnlyObjects;
        
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
                vrOnlyObjects.ForEach(o => o.SetActive(true));
                kbmOnlyObjects.ForEach(o => o.SetActive(false));
                StartCoroutine(StartXR());
            }
            
            else
            {
                vrOnlyObjects.ForEach(o => o.SetActive(false));
                kbmOnlyObjects.ForEach(o => o.SetActive(true));
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