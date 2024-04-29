using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.UI;
using UnityEngine;

public class UserInterfaceManager : MonoBehaviour
{
    public static UserInterfaceManager inst;
    public SceneManager sceneMgr;
    public GameManager gameMgr;
    public MainMenuController mainMenuController;
    public OptionsMenuController optionsMenuController;
    public EscapeMenuController escapeMenuController;

    public GameObject canvas;
    public GameObject background;
    public GameObject escapeMenuBackground;
    public GameObject loadingScreen;
    public GameObject optionsMenu;
    public GameObject mainMenu;
    public GameObject escapeMenu;
    public GameObject keyRebindingMenu;
    public GameObject gameOverMenu;
    public GameObject gameWonMenu;
    public GameObject playerGameUI;
    public GameObject playerDamageScreen;
    public PlayerHealthBar healthBar;
    public UnityEngine.UI.Slider progressBar;

    void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        background = GameObject.Find("Background");
        escapeMenuBackground = GameObject.Find("EscapeMenuBackground");
        mainMenu = GameObject.Find("MainMenu");
        escapeMenu = GameObject.Find("EscapeMenu");
        keyRebindingMenu = GameObject.Find("KeyRebindingMenu");
        optionsMenu = GameObject.Find("OptionsMenu");
        loadingScreen = GameObject.Find("LoadingScreen");
        gameOverMenu = GameObject.Find("GameOverMenu");
        gameWonMenu = GameObject.Find("GameWinMenu");
        playerGameUI = GameObject.Find("PlayerGameUI");
        playerDamageScreen = GameObject.Find("PlayerDamageScreen");
        progressBar = GameObject.Find("ProgressBar").GetComponent<Slider>();
        healthBar = GameObject.Find("Health bar").GetComponent<PlayerHealthBar>();

        mainMenuController = mainMenu.GetComponent<MainMenuController>();
        optionsMenuController = optionsMenu.GetComponent<OptionsMenuController>();
        escapeMenuController = inst.GetComponent<EscapeMenuController>();
        mainMenuController.setUserInterfaceManager(inst);
        optionsMenuController.setUserInterfaceManager(inst);
        escapeMenuController.setUserInterfaceManager(inst);

        EnableMenuElement(mainMenu);

        sceneMgr = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
        sceneMgr.setUserInterfaceManager(inst);
        gameMgr.setUserInterfaceManager(inst);
    }

    public void MainMenu()
    {
        EnableMenuElement(mainMenu);
    }

    public void OptionsMenu()
    {
        EnableMenuElement(optionsMenu);
        // optionsMenuController.UpdateOptions();
    }

    public void OptionsMenuPaused()
    {
        EnableMenuElement(optionsMenu, false);
        // optionsMenuController.UpdateOptions();
        escapeMenuController.LeftEscapeMenu();
    }

    public void OptionsMenuBackPaused()
    {
        EnableMenuElement(escapeMenu, false);
        escapeMenuController.ReturnedToEscapeMenu();
    }

    public void GameLoading()
    {
        EnableMenuElement(loadingScreen);
    }

    public void GameWon()
    {
        EnableMenuElement(gameWonMenu);
    }

    public void GameOver()
    {
        EnableMenuElement(gameOverMenu);
    }

    public void BackToMainMenu()
    {
        EnableMenuElement(mainMenu);
        gameMgr.BackToMainMenu();
    }

    public void KeyRebinding()
    {
        EnableMenuElement(keyRebindingMenu);
    }

    public void KeyRebindingBackButton()
    {
        // EnableMenuElement(optionsMenu);
        if (gameMgr.isGamePaused)
        {
            EnableMenuElement(optionsMenu, false);
        }
        else
        {
            EnableMenuElement(optionsMenu);
        }
    }

    public void EscapeMenu()
    {
        gameMgr.PauseGame();
        EnableMenuElement(escapeMenu, false);
    }

    public void ResumeGame()
    {
        canvas.SetActive(true);
        background.SetActive(false);
        escapeMenuBackground.SetActive(false);
        escapeMenu.SetActive(false);
        optionsMenu.SetActive(false);
        loadingScreen.SetActive(false);
        keyRebindingMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        gameWonMenu.SetActive(false);
        playerDamageScreen.SetActive(false);
        playerGameUI.SetActive(true);

        gameMgr.ResumeGame();
    }

    public void GameStart()
    {
        loadingScreen.SetActive(false);
        background.SetActive(false);
        playerGameUI.SetActive(true);
    }

    public void DisplayDamage(float health)
    {
        // canvas.SetActive(true);
        Debug.Log("Damage!");
        healthBar.SetHealth(health);
    }

    public void HideDamage()
    {
        // canvas.SetActive(false);
    }

    public void DisplayWeaponBuff(float amount)
    {
        // TODO : implement visual display of weapon speed/cooldown powerup
        Debug.Log("UI Mgr Weapon Buff");
    }

    public void DisplayWeaponDamageBuff(float amount)
    {
        // TODO : implement visual display of weapon damage powerup
        Debug.Log("UI Mgr Weapon Damage Buff");
    }

    public void DisplayMaxHealthBuff(float maxHealth)
    {
        // TODO : implement visual display of max health increase
    }

    public void DisplayHealing(float health)
    {
        healthBar.SetHealth(health);
    }

    public void DisplayPlayerDefenseBuff(float amount)
    {
        // TODO : implement visual display of absolute defense powerup
    }

    public void DisplayPlayerSpeedBuff(float amount)
    {
        // TODO : implement visual display of move speed powerup
    }

    public void DisplayPlayerDashBuff(float amount)
    {
        // TODO : implement visual display of dash cooldown powerup
    }

    public void DisplayPlayerJumpBuff(float amount)
    {
        // TODO : implement visual display of jump force powerup
    }

    public void updateProgressBar(float progress)
    {
        progressBar.value = progress;
    }

    public void LoadFirstLevel()
    {
        sceneMgr.LoadSceneByName(gameMgr.firstLevelName);
    }

    public void EnableMenuElement(GameObject element, bool enableFullBackground = true)
    {
        canvas.SetActive(true);
        background.SetActive(enableFullBackground);
        escapeMenuBackground.SetActive(!enableFullBackground);


        mainMenu.SetActive(false);
        escapeMenu.SetActive(false);
        optionsMenu.SetActive(false);
        loadingScreen.SetActive(false);
        keyRebindingMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        gameWonMenu.SetActive(false);
        playerDamageScreen.SetActive(false);
        playerGameUI.SetActive(false);

        element.SetActive(true);
    }

    public void QuitGame()
    {
        // Quit the game. This will only work in the built game, not in the editor
        Application.Quit();
    }
}
