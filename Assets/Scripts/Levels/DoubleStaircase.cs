using PerceptionVR.Physics;
using PerceptionVR.Portal;
using UnityEngine;
using UnityEngine.Serialization;

namespace PerceptionVR.Levels
{
    public class DoubleStaircase : LevelBase
    { 
        [SerializeField] private SubscribableTrigger loopHallwayTrigger;
        [SerializeField] private SubscribableTrigger blueTrigger;
        [SerializeField] private SubscribableTrigger redTrigger;
        
        [SerializeField] private Portal.Portal bluePortal;
        [SerializeField] private Portal.Portal redPortal;
        [SerializeField] private Portal.Portal loopHallwayPortal;
        
        [SerializeField] private Portal.Portal gravityFlipEntrancePortal;
        [SerializeField] private Portal.Portal gravityFlipExitPortal;
            
        [SerializeField] private SubscribableTrigger entranceTrigger;
        [SerializeField] private Portal.Portal entrancePortal;

        private void Awake()
        {
            //Portal.Portal.SetPortalPair(entrancePortal, loopHallwayPortal);


            blueTrigger.onTriggerEnter += other =>
            {
                if (other.transform.name.Contains("Player"))
                    Portal.Portal.SetPortalPair(loopHallwayPortal, bluePortal);
            };

            redTrigger.onTriggerEnter += other =>
            {
                if (other.transform.name.Contains("Player"))
                    Portal.Portal.SetPortalPair(loopHallwayPortal, redPortal);
            };

            loopHallwayTrigger.onTriggerEnter += other =>
            {
                if (other.transform.name.Contains("Player"))
                {
                    Portal.Portal.SetPortalPair(loopHallwayPortal, gravityFlipEntrancePortal);
                    Portal.Portal.SetPortalPair(gravityFlipExitPortal, entrancePortal);
                }   
            };

            entranceTrigger.onTriggerEnter += other =>
            {
                if (other.transform.name.Contains("Player"))
                    Portal.Portal.SetPortalPair(entrancePortal, loopHallwayPortal);
            };
        }
    }
}


