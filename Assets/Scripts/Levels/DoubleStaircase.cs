using PerceptionVR.Physics;
using PerceptionVR.Portals;
using UnityEngine;
using UnityEngine.Serialization;

namespace PerceptionVR.Levels
{
    public class DoubleStaircase : LevelBase
    { 
        [SerializeField] private SubscribableTrigger loopHallwayTrigger;
        [SerializeField] private SubscribableTrigger blueTrigger;
        [SerializeField] private SubscribableTrigger redTrigger;
        
        [SerializeField] private Portal bluePortal;
        [SerializeField] private Portal redPortal;
        [SerializeField] private Portal loopHallwayPortal;
        
        [SerializeField] private Portal gravityFlipEntrancePortal;
        [SerializeField] private Portal gravityFlipExitPortal;
            
        [SerializeField] private SubscribableTrigger entranceTrigger;
        [SerializeField] private Portal entrancePortal;

        private void Awake()
        {
            blueTrigger.onTriggerEnter += other =>
            {
                if (other.transform.name.Contains("Player"))
                    Portal.SetPortalPair(loopHallwayPortal, bluePortal);
            };

            redTrigger.onTriggerEnter += other =>
            {
                if (other.transform.name.Contains("Player"))
                    Portal.SetPortalPair(loopHallwayPortal, redPortal);
            };

            loopHallwayTrigger.onTriggerEnter += other =>
            {
                if (other.transform.name.Contains("Player"))
                {
                    Portal.SetPortalPair(loopHallwayPortal, gravityFlipEntrancePortal);
                    Portal.SetPortalPair(gravityFlipExitPortal, entrancePortal);
                }   
            };

            entranceTrigger.onTriggerEnter += other =>
            {
                if (other.transform.name.Contains("Player"))
                    Portal.SetPortalPair(entrancePortal, loopHallwayPortal);
            };
        }
    }
}


