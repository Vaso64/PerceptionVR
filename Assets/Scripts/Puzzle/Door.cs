using System;
using PerceptionVR.Extensions;
using UnityEngine;

namespace PerceptionVR.Puzzle
{
    [RequireComponent(typeof(Collider))]
    public class Door : MonoBehaviourBase
    {
        private new Collider collider;
        private MeshRenderer meshRenderer;
        
        public bool isOpen { get; private set; }
        
        private void Awake()
        {
            collider = GetComponent<Collider>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void Open()
        {
            if(isOpen)
                return;
            
            collider.enabled = false;
            meshRenderer.enabled = false;
            isOpen = true;
        }
        
        public void Close()
        {
            if(!isOpen)
                return;
            
            collider.enabled = true;
            meshRenderer.enabled = true;
            isOpen = false;
        }
        
        public void SetOpen(bool open)
        {
            if(open) Open();
            else Close();
        }
    }
}