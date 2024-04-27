using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public UserInterfaceManager userInterfaceMgr;
    public SceneManager sceneMgr;

    public string firstLevelName = "PrototypeLevel";

    public bool isGamePaused = false;
    public bool gameplayStarted = false;

    private void Awake()
    {
        inst = this;
    }

    private QuestManager questMgr;
    private PlayerManager playerMgr;

    public int numLevels;       // number of levels in the game
    public int numLevelsCompleted;      //  number of levels player has completed

    void Start()
    {
        sceneMgr = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        sceneMgr.LoadPlayerAndUserInterfaceScenes();
        sceneMgr.setGameManager(inst);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setUserInterfaceManager(UserInterfaceManager uim)
    {
        userInterfaceMgr = uim;
    }

    public void DisplayDamage(float health)
    {
        userInterfaceMgr.DisplayDamage(health);
    }

    public void HideDamage()
    {
        userInterfaceMgr.HideDamage();
    }

    public void GameOver()
    {
        userInterfaceMgr.GameOver();
    }

    public void GameWon()
    {
        userInterfaceMgr.GameWon();
    }

    public void GameStart()
    {
        userInterfaceMgr.GameStart();

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        gameplayStarted = true;

        // invoke player camera and set camera orientation
    }

    public void BackToMainMenu()
    {
        gameplayStarted = false;

        // Unload the current level with scene manager
    }

    public void PauseGame()
    {
        isGamePaused = true;
        
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        // Stop player movement
        // Stop camera movement


    }

    public void ResumeGame()
    {
        isGamePaused = false;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        // Resume player movement
        // Resume camera movement
    }
}
