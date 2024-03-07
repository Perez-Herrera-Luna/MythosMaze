using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public SceneManager sceneMgr;
    public MainMenuController inst;
    public string firstLevelName;

    public void Start()
    {
        inst = this;
        sceneMgr = GameObject.Find("SceneManager").GetComponent<SceneManager>();
    }

    public void PlayGame()
    {
        // Invoke scene loader to async load the game scene
        sceneMgr.LoadSceneByName(firstLevelName);
        gameObject.SetActive(false);
        
    }

    public void QuitGame()
    {
        // Quit the game. This will only work in the built game, not in the editor
        Application.Quit();
    }
}
