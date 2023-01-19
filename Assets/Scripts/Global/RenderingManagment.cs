using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using System;

namespace PerceptionVR.Global
{
    public class RenderingManagment : MonoBehaviour
    {
        public static Action<Vector2Int> OnResolutionChange;
        public static Vector2Int currentResolution = new Vector2Int(0, 0);

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(ResolutionTracker());
        }


        IEnumerator ResolutionTracker(){
            while(true){
                // VR
                if(XRSettings.enabled && false){
                    if(currentResolution.x != XRSettings.eyeTextureWidth || currentResolution.y !=  XRSettings.eyeTextureHeight){
                        currentResolution = new Vector2Int(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight);
                        OnResolutionChange?.Invoke(currentResolution);
                    }
                }

                // Flat
                else{
                    if(currentResolution.x != Screen.width || currentResolution.y != Screen.height){
                        currentResolution = new Vector2Int(Screen.width, Screen.height);
                        OnResolutionChange?.Invoke(currentResolution);
                    }
                }

                // Check every second
                yield return new WaitForSeconds(1f);
            }
        }
    }
}


