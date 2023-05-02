using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        public static void ClipByBounds(this MeshFilter meshFilter, Bounds bounds)
        {
            foreach (var plane in bounds.GetPlanes())
                meshFilter.ClipByPlane(plane.flipped);
        }

        // Note: Unknown bug when clipping scaled mesh
        public static void ClipByPlane(this MeshFilter meshFilter, Plane plane)
        {
            var oldMeshVertices = meshFilter.mesh.vertices;
            var oldMeshTriangles = meshFilter.mesh.triangles;
            plane = new Plane(meshFilter.transform.InverseTransformDirection(plane.normal), 
                              meshFilter.transform.InverseTransformPoint(plane.normal * -plane.distance));

            var newMeshVertices = new List<Vector3>();
            var planeEdges = new List<(Vector3, Vector3)>();
            
            var vertices = new Vector3[3];
            int positiveIndex = 0, negativeIndex = 0, triangleCount = 0;

            // Iterate through all triangles
            for (var i = 0; i < oldMeshTriangles.Length; i += 3)
            {
                var positiveCount = 0;
                for (var k = 0; k < 3; k++)
                {
                    vertices[k] = oldMeshVertices[oldMeshTriangles[i + k]];
                    if (plane.GetSide(vertices[k]))
                    {
                        positiveCount++;
                        positiveIndex = k;
                    }
                    else
                        negativeIndex = k;
                }


                switch (positiveCount)
                {
                    // One point is on the positive side
                    case 1:
                        // Create new triangle
                        newMeshVertices.Add(vertices[positiveIndex]);
                        newMeshVertices.Add(plane.TwoPointIntersection(vertices[positiveIndex], vertices[(positiveIndex+1)%3]));
                        newMeshVertices.Add(plane.TwoPointIntersection(vertices[positiveIndex], vertices[(positiveIndex+2)%3]));
                        planeEdges.Add((newMeshVertices[^1], newMeshVertices[^2]));
                        triangleCount++;
                        break;
                    
                    // Two points are on the positive side
                    case 2:
                        // Create first new triangle
                        newMeshVertices.Add(plane.TwoPointIntersection(vertices[negativeIndex], vertices[(negativeIndex+1)%3]));
                        newMeshVertices.Add(vertices[(negativeIndex + 1) % 3]);
                        newMeshVertices.Add(vertices[(negativeIndex + 2) % 3]);

                        // Create second new triangle
                        newMeshVertices.Add(newMeshVertices[^3]);
                        newMeshVertices.Add(vertices[(negativeIndex + 2) % 3]);
                        newMeshVertices.Add(plane.TwoPointIntersection(vertices[negativeIndex], vertices[(negativeIndex+2)%3]));
                        planeEdges.Add((newMeshVertices[^3], newMeshVertices[^1]));
                        
                        triangleCount += 2;
                        break;
                    
                    // All points are on the positive side
                    case 3:
                        // Copy existing triangle
                        newMeshVertices.AddRange(vertices);
                        triangleCount++;
                        break;
                }
            }
            
            // Fill cut mesh
            var centerPoint = planeEdges.Aggregate(Vector3.zero, (current, edge) => current + edge.Item1 + edge.Item2) / (planeEdges.Count * 2);
            for (var i = 0; i < planeEdges.Count; i++)
            {
                newMeshVertices.Add(planeEdges[i].Item1);
                newMeshVertices.Add(planeEdges[i].Item2);
                newMeshVertices.Add(centerPoint);
                triangleCount++;
            }
            
            meshFilter.mesh.Clear();
            meshFilter.mesh.vertices = newMeshVertices.ToArray();
            meshFilter.mesh.triangles = Enumerable.Range(0, triangleCount * 3).ToArray();

            meshFilter.mesh.RecalculateNormals();
            meshFilter.mesh.RecalculateBounds();
            meshFilter.mesh.Optimize();
        }
    }
}