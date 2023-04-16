using System;
using PerceptionVR.Common;

namespace PerceptionVR.Portals
{
    public interface ISwappable : IMonoBehaviour
    {
        public Action<SwapData> OnSwap { get; set; }
    }
}