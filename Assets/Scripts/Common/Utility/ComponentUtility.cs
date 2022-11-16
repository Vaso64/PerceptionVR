using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PerceptionVR.Debug;

namespace PerceptionVR.Common
{
    public static class ComponentUtility
    {
        public static IEnumerable<(TOriginal original, TClone clone)> CreateComponentTuple<TOriginal, TClone>(Component originalRoot, Component cloneRoot) 
        {
            var originalComponents = originalRoot.GetComponentsInChildren<TOriginal>();
            var cloneComponents = cloneRoot.GetComponentsInChildren<TClone>();
            if(originalComponents.Length != cloneComponents.Length)
                Debugger.LogError($"{originalRoot} and {cloneRoot} have different numbers of {typeof(TOriginal)} / {typeof(TClone)} components");
            return originalComponents.Zip(cloneComponents, (original, clone) => (original, clone));
        }
    }
}