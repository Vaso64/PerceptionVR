namespace PerceptionVR.Puzzle
{
    public class TogglePressureButton : PressureButtonBase
    {
        protected override void Update()
        {
            base.Update();
            if(!wasPressedLastFrame && isPressedThisFrame)
                SetActive(!isActivated);
        }   
    }
}