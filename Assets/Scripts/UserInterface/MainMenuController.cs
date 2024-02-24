using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        // Invoke scene loader to async load the game scene

    }

    public void QuitGame()
    {
        // Quit the game. This will only work in the built game, not in the editor
        Application.Quit();
    }
}
