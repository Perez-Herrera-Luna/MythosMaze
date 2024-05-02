using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public UserInterfaceManager userInterfaceMgr;
    // public SceneManager sceneMgr;

    public string firstLevelName = "Level1";

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

    // private QuestManager questMgr;
    // private PlayerManager playerMgr;

    public int numLevels;       // number of levels in the game
    public int numLevelsCompleted;      //  number of levels player has completed

    void Start()
    {
        // sceneMgr = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        SceneManager.inst.LoadPlayerAndUserInterfaceScenes();
        // sceneMgr.setGameManager(inst);
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

    public void DisplayWeaponPowerup(float amount)
    {
        if (amount < 1)
            userInterfaceMgr.DisplayWeaponBuff(amount);
        else
            userInterfaceMgr.DisplayWeaponDamageBuff(amount);
    }
    
    public void DisplayPlayerPowerup(string type, float amount)
    {
        switch (type)
        {
            case "defense":
                userInterfaceMgr.DisplayPlayerDefenseBuff(amount);
                break;

            case "speed":
                userInterfaceMgr.DisplayPlayerSpeedBuff(amount);
                break;

            case "dash":
                userInterfaceMgr.DisplayPlayerDashBuff(amount);
                break;

            case "jump":
                userInterfaceMgr.DisplayPlayerJumpBuff(amount);
                break;

            default:
                Debug.Log("Error: Invalid type of player powerup");
                break;
        }
    }

    public void DisplayMaxHealthBuff(float maxHealth)
    {
        userInterfaceMgr.DisplayMaxHealthBuff(maxHealth);
    }

    public void DisplayPlayerHealing(float health)
    {
        userInterfaceMgr.DisplayHealing(health);
    }

    public void GameOver()
    {
        userInterfaceMgr.GameOver();
        
        isGamePaused = true;
        
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        InputManager.instance.DisableMovementInput();
        InputManager.instance.DisableCameraInput();

        // SceneManager.inst.UnloadCurrLevel();
    }

    public void GameWon()
    {
        userInterfaceMgr.GameWon();

        isGamePaused = true;
        
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        InputManager.instance.DisableMovementInput();
        InputManager.instance.DisableCameraInput();
    }

    public void GameStart()
    {
        userInterfaceMgr.GameStart();
        AudioManager.inst.PlayBackgroundTrack();

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        gameplayStarted = true;

        // invoke player camera and set camera orientation
    }

    public void BackToMainMenu()
    {
        gameplayStarted = false;

        ResumeMainMenu();
        SceneManager.inst.UnloadCurrLevel();
    }

    public void PauseGame()
    {
        isGamePaused = true;
        
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        InputManager.instance.DisableMovementInput();
        InputManager.instance.DisableCameraInput();

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        isGamePaused = false;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        InputManager.instance.EnableMovementInput();
        InputManager.instance.EnableCameraInput();

        Time.timeScale = 1;
    }

    public void ResumeMainMenu()
    {
        isGamePaused = false;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        InputManager.instance.EnableMovementInput();
        InputManager.instance.EnableCameraInput();

        Time.timeScale = 1;

        PlayerManager.inst.resetPlayer();
    }
}
