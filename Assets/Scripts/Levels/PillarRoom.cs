using System.Collections;
using PerceptionVR.Levels;
using PerceptionVR.Puzzle;
using UnityEngine;

public class PillarRoom : LevelBase
{
    [SerializeField] private Button pressurePlate;
    [SerializeField] private Door door;
    [SerializeField] private float doorCloseTimeout;
    
    private Coroutine closeDoorCoroutine;
    
    private void Awake()
    {
        pressurePlate.OnPressed.AddListener(OnActivateCallback);
        pressurePlate.OnReleased.AddListener(OnDeactivateCallback);
    }

    private void OnActivateCallback()
    {
        pressurePlate.SetColor(Color.green);
        if(closeDoorCoroutine != null)
            StopCoroutine(closeDoorCoroutine);
        door.Open();
    }
    
    private void OnDeactivateCallback() => closeDoorCoroutine = StartCoroutine(CloseDoorTimeout());

    private IEnumerator CloseDoorTimeout()
    {
        pressurePlate.SetColor(Color.yellow);
        yield return new WaitForSeconds(doorCloseTimeout);
        pressurePlate.SetColor(Color.red);
        door.Close();
    }
}
