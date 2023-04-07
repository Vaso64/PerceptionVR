using PerceptionVR.Extensions;
using UnityEngine;

namespace PerceptionVR.Puzzle
{
    [RequireComponent(typeof(MeshFilter))]
    public class Indicator : MonoBehaviourBase
    {
        private MeshFilter meshFilter;
        
        private void Awake() => meshFilter = GetComponent<MeshFilter>();

        public void SetColor(Color color) => meshFilter.mesh.SetVertexColor(color);
    }
}