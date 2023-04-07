using System;
using System.Collections;
using PerceptionVR.Levels;
using PerceptionVR.Puzzle;
using UnityEngine;

public class PillarRoom : LevelBase
{
    [SerializeField] private Button pressurePlate;
    [SerializeField] private Door door;
    
    private Coroutine closeDoorCoroutine;
    
    private void Awake()
    {
        pressurePlate.OnPressed.AddListener(OnActivateCallback);
        pressurePlate.OnReleased.AddListener(OnDeactivateCallback);
    }

    private void OnActivateCallback()
    {
        if(closeDoorCoroutine != null)
            StopCoroutine(closeDoorCoroutine);
        door.Open();
    }
    
    private void OnDeactivateCallback() => closeDoorCoroutine = StartCoroutine(CloseDoorTimeout());

    private IEnumerator CloseDoorTimeout()
    {
        yield return new WaitForSeconds(7f);
        door.Close();
    }
}
