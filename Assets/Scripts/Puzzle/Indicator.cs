using System;
using PerceptionVR.Extensions;
using UnityEngine;

namespace PerceptionVR.Puzzle
{
    [RequireComponent(typeof(MeshFilter))]
    public class Indicator : ControlBase
    {
        private MeshFilter meshFilter;
        
        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            OnChanged.AddListener(OnChangedCallback);
        }
        
        private void OnChangedCallback(bool value) => meshFilter.mesh.SetVertexColor(value ? Color.green : Color.red);
    }
}