using PerceptionVR.Physics;
using UnityEngine;

namespace PerceptionVR.Levels
{
    public class DoubleStaircase : LevelBase
    {
        [SerializeField] private SubscribableTrigger mainHallwayTrigger;
        [SerializeField] private SubscribableTrigger blueStaircaseTrigger;
        [SerializeField] private SubscribableTrigger redStaircaseTrigger;
        
        [SerializeField] private Portal.Portal blueStaircasePortal;
        [SerializeField] private Portal.Portal redStaircasePortal;
        [SerializeField] private Portal.Portal mainHallwayPortal;
        [SerializeField] private Portal.Portal exitPortalInside;
        [SerializeField] private Portal.Portal exitPortalOutside;
        [SerializeField] private Portal.Portal entrancePortal;

        private void Awake()
        {
            //PortalBase.SetPortalPair(entrancePortal, mainHallwayPortal);

            blueStaircaseTrigger.onTriggerEnter += (other) =>
            {
                if(other.transform.name.Contains("Player"))
                    Portal.Portal.SetPortalPair(mainHallwayPortal, blueStaircasePortal);
            };
            
            redStaircaseTrigger.onTriggerEnter += (other) =>
            {
                if(other.transform.name.Contains("Player"))
                    Portal.Portal.SetPortalPair(mainHallwayPortal, redStaircasePortal);
            };
            
            mainHallwayTrigger.onTriggerEnter += (other) =>
            {
                if (other.transform.name.Contains("Player"))
                {
                    Portal.Portal.SetPortalPair(mainHallwayPortal, exitPortalInside);
                    Portal.Portal.SetPortalPair(entrancePortal, exitPortalOutside);
                }
                    
            };
        }
    }
}


