using System;
using PerceptionVR.Extensions;
using UnityEngine;

namespace PerceptionVR.Puzzle
{
    [RequireComponent(typeof(Collider))]
    public class Door : ControlBase
    {
        private void Awake() => OnChanged.AddListener(active =>
        {
            GetComponent<Collider>().enabled = active;
            GetComponent<MeshRenderer>().enabled = active;
        });
    }
}