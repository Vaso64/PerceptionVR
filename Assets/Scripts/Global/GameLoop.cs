using UnityEngine;

public class GameLoop: MonoBehaviourBase
{
    private void Start()
    {
        var kbmInput = new PlayerInputAction().KBMPlayer;
        kbmInput.Enable();
        //kbmInput.Reset.performed += _ => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        kbmInput.Exit.performed += _ => Application.Quit();
    }
}
