namespace PerceptionVR.Puzzle
{
    public class HoldPressureButton : PressureButtonBase
    {
        protected override void Update()
        {
            base.Update();
            if(isActivated != isPressedThisFrame)
                SetActive(isPressedThisFrame);
        }
    }
}