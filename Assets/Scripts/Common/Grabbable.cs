using PerceptionVR.Common;
using PerceptionVR.Physics;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public new Collider collider { get; private set; }
    public PhysicsObject physicsObject { get; private set; }
    
    private void Awake()
    {
        collider = GetComponent<Collider>();
        physicsObject = GetComponent<PhysicsObject>();
    }
}
