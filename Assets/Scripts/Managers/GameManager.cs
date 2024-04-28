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
        if(inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        sceneMgr.UnloadCurrLevel();
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

        ResumeMainMenu();
        sceneMgr.UnloadCurrLevel();
    }

    public void PauseGame()
    {
        isGamePaused = true;
        
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        InputManager.instance.DisableMovementInput();
        InputManager.instance.DisableCameraInput();
    }

    public void ResumeGame()
    {
        isGamePaused = false;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        InputManager.instance.EnableMovementInput();
        InputManager.instance.EnableCameraInput();
    }

    public void ResumeMainMenu()
    {
        isGamePaused = false;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        InputManager.instance.EnableMovementInput();
        InputManager.instance.EnableCameraInput();
    }
}
