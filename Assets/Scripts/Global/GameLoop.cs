using PerceptionVR.Debug;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PerceptionVR.Global
{
    public class GameLoop: MonoBehaviourBase
    {
        private void Start()
        {
            var kbmInput = new PlayerInputAction().KBMPlayer;
            kbmInput.Enable();
            //kbmInput.Reset.performed += _ => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            kbmInput.Exit.performed += _ => Application.Quit();
        }

        public static void ResetGame()
        {
            Debugger.LogInfo("Resetting game...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
