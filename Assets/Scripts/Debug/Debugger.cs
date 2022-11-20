using System;
using UnityEngine;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PerceptionVR.Debug
{
    public static class Debugger
    {
        private const bool LogEnabled = true;

        public static void LogInfo(string message) {
            if(LogEnabled)
                UnityEngine.Debug.Log($"{Header()} {message}");
        } 
        public static void LogWarning(string message)
        { 
            if(LogEnabled)
                UnityEngine.Debug.LogWarning($"{Header()} {message}");
        }
        public static void LogError(string message)
        {
            if(LogEnabled)
                UnityEngine.Debug.LogError($"{Header()} {message}");
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