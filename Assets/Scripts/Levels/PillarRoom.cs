using System;
using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Levels;
using PerceptionVR.Puzzle;
using UnityEngine;

public class PillarRoom : LevelBase
{
    [SerializeField] private HoldPressureButton pressurePlate;
    [SerializeField] private Door door;
    
    private Coroutine closeDoorCoroutine;
    
    private void Awake()
    {
        pressurePlate.OnActivated.AddListener(OnActivateCallback);
        pressurePlate.OnDeactivated.AddListener(OnDeactivateCallback);
    }

    private void OnActivateCallback()
    {
        if(closeDoorCoroutine != null)
            StopCoroutine(closeDoorCoroutine);
        door.SetActive(false);
    }
    
    private void OnDeactivateCallback()
    {
        closeDoorCoroutine = StartCoroutine(CloseDoor());
    }
    
    private IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(5f);
        door.SetActive(true);
    }
}
