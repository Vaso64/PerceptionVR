using UnityEngine;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PerceptionVR.Debug
{
    public class Debugger: MonoBehaviourBase
    {
        [SerializeField] private LogLevel logLevel;
        [SerializeField] private bool frameSplit;

        private void Awake() => _logLevel = logLevel;
        private void Update() => _logLevel = logLevel;
        
               
        private static LogLevel _logLevel;
        private static bool _loggedThisFrame;


        public static void LogInfo(string message) 
        {
            if (_logLevel >= LogLevel.Info)
            {
                _loggedThisFrame = true;
                UnityEngine.Debug.Log($"{Header()} {message}");
            }
        } 
        
        public static void LogWarning(string message)
        {
            if (_logLevel >= LogLevel.Warning)
            {
                _loggedThisFrame = true;
                UnityEngine.Debug.LogWarning($"{Header()} {message}");
            }
        }
        
        public static void LogError(string message)
        {
            if (_logLevel >= LogLevel.Error)
            {
                _loggedThisFrame = true;
                UnityEngine.Debug.LogError($"{Header()} {message}");
            }
        }

        private void LateUpdate()
        {
            if (frameSplit && _loggedThisFrame)
            {
                UnityEngine.Debug.Log("= = = = = = = = = = E N D   O F   F R A M E = = = = = = = = = =");
                _loggedThisFrame = false;
            }
        }

        private static string Header()
        {
            var header = "";
            
            // Frame count
            header += $"[{Time.frameCount:D5}]";
        
            // Caller
            var stack = new StackTrace();
            var frame = stack.GetFrame(2);
            var method = frame.GetMethod();
            var name = new Regex("(([a-zA-Z]+\\.)*)([a-zA-Z]+).*").Match(method.DeclaringType.FullName).Groups[3].Value;
            header += $" [{name}]";
            
            // Padding
            header = header.PadRight(40);

            return header;
        }
    }
}