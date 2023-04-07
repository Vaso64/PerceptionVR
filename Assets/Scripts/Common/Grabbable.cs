using PerceptionVR.Common;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public Collider collider { get; private set; }
    public Rigidbody rigidbody { get; private set; }
    
    private void Awake()
    {
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
    }
}
