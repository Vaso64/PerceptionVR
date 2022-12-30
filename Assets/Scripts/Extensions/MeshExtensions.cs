using UnityEngine;
using System;

namespace PerceptionVR.Extensions
{
    public static class MeshExtensions
    {
        public static void SetVertexColor(this Mesh mesh, Color color)
        {
            var colors = new Color[mesh.vertexCount];
            for (var i = 0; i < colors.Length; i++)
                colors[i] = color;
            mesh.colors = colors;
        }
    }
}