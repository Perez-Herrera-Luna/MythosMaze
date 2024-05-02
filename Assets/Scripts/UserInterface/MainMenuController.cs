using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public UserInterfaceManager userInterfaceMgr;
    public MainMenuController inst;

    public void Start()
    {
        inst = this;
    }

    public void setUserInterfaceManager(UserInterfaceManager uim)
    {
        userInterfaceMgr = uim;
    }

    public void PlayGame()
    {
        // Invoke scene loader to async load the game scene
        userInterfaceMgr.LoadFirstLevel();
        gameObject.SetActive(false);

        AudioManager.inst.PlayMenuInteraction();
    }

    public void OptionsMenu()
    {
        userInterfaceMgr.OptionsMenu();
    }
}
