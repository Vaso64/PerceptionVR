using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using PerceptionVR.Levels;
using PerceptionVR.Portals;
using PerceptionVR.Puzzle;
using UnityEngine;

public class MirrorRooms : LevelBase
{
    [System.Serializable] private class Room
    {
        public List<Portal> portals;
        public Button button;
        public Indicator indicator;
        public bool wasPressed;
    }

    [SerializeField] private Door exitDoor;
    [SerializeField] private List<Room> rooms = new();

    private void Start()
    {
        // Get list of unconnected portals and rooms
        var notConnectedRooms = rooms.ToList();
        var notConnectedPortal = notConnectedRooms.SelectMany(x => x.portals).ToList();
        if (notConnectedPortal.Count % 2 != 0)
            Debug.LogWarning("Uneven number of total portals.");

        // Connect one portal from each room to a random portal in the other room so all rooms are connected
        var currentRoom = notConnectedRooms[Random.Range(0, notConnectedRooms.Count)];
        notConnectedRooms.Remove(currentRoom);
        while (notConnectedRooms.Count > 0)
        {
            var nextRoom = notConnectedRooms[Random.Range(0, notConnectedRooms.Count)];
            notConnectedRooms.Remove(nextRoom);
            
            // Pick two unpaired portals
            var currentRoomPortal = currentRoom.portals.Shuffle().First(x => notConnectedPortal.Contains(x)); notConnectedPortal.Remove(currentRoomPortal);
            var nextRoomPortal =       nextRoom.portals.Shuffle().First(x => notConnectedPortal.Contains(x)); notConnectedPortal.Remove(nextRoomPortal);
            Portal.SetPortalPair(currentRoomPortal, nextRoomPortal);
            currentRoom = nextRoom;
        }
        
        // Connect rest of the portals
        while (notConnectedPortal.Count > 1)
        {
            // Pick two random portals and pair them
            var randomPortal1 = notConnectedPortal[Random.Range(0, notConnectedPortal.Count)]; notConnectedPortal.Remove(randomPortal1);
            var randomPortal2 = notConnectedPortal[Random.Range(0, notConnectedPortal.Count)]; notConnectedPortal.Remove(randomPortal2);
            Portal.SetPortalPair(randomPortal1, randomPortal2);
        }

        // Add listeners to buttons
        foreach (var room in rooms)
        {
            room.button.OnPressed.AddListener(() => OnButtonPressed(room));
            room.button.OnChanged.AddListener(active => room.button.SetColor(active ? Color.green : Color.red));
        }
    }

    private void OnButtonPressed(Room pressedButtonRoom)   
    {
        // If button was already pressed => Reset
        if (pressedButtonRoom.wasPressed) foreach (var room in rooms)
        {
            room.wasPressed = false;
            room.indicator.SetColor(Color.red);
            exitDoor.Close();
        }
        // If not => Set button to pressed
        else
        {
            pressedButtonRoom.wasPressed = true;
            pressedButtonRoom.indicator.SetColor(Color.green);
        }
        
        // If all buttons are pressed => Open exit door
        if (rooms.All(room => room.wasPressed))
            exitDoor.Open();
    }
}
