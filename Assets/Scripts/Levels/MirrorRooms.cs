using System.Collections.Generic;
using PerceptionVR.Levels;
using PerceptionVR.Portals;
using PerceptionVR.Puzzle;
using UnityEngine;

public class MirrorRooms : LevelBase
{
    [SerializeField] private List<Portal> randomPortalPool;
    [SerializeField] private List<Button> buttons;
    [SerializeField] private List<Indicator> indicators;
    [SerializeField] private Door exitDoor;
    private readonly List<Button> pressedButtons = new ();

    private void Start()
    {
        if (randomPortalPool.Count % 2 != 0)
            Debug.LogWarning("Uneven number of portals in pool.");
        
        // Randomly connect portals
        var randomPortalPoolCopy = new List<Portal>(randomPortalPool);
        while (randomPortalPoolCopy.Count > 1)
        {
            // Pick two random portals from the pool
            var randomPortal1 = randomPortalPoolCopy[Random.Range(0, randomPortalPoolCopy.Count)];
            randomPortalPoolCopy.Remove(randomPortal1);
            var randomPortal2 = randomPortalPoolCopy[Random.Range(0, randomPortalPoolCopy.Count)];
            randomPortalPoolCopy.Remove(randomPortal2);
            
            // Connect them
            Portal.SetPortalPair(randomPortal1, randomPortal2);
        }

        foreach (var button in buttons)
        {
            button.OnPressed.AddListener(() => OnButtonPressed(button));
            button.OnChanged.AddListener(active => button.SetColor(active ? Color.green : Color.red));
        }
            
    }

    private void OnButtonPressed(Button button)    
    {
        // If button was already pressed => Reset indicators
        if (pressedButtons.Contains(button))
        {
            pressedButtons.Clear();
            foreach (var indicator in indicators)
                indicator.SetColor(Color.red);
            exitDoor.Close();
        }
        
        // If not => Light up indicator
        else
        {
            pressedButtons.Add(button);
            indicators[pressedButtons.IndexOf(button)].SetColor(Color.green);
        }
        
        // If all indicators are lit up
        if (pressedButtons.Count == buttons.Count)
            exitDoor.Open();
    }
}
