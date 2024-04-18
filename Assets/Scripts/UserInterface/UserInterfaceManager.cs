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

    public GameObject canvas;
    public GameObject background;
    public GameObject loadingScreen;
    public GameObject optionsMenu;
    public GameObject mainMenu;
    public GameObject escapeMenu;
    public GameObject keyRebindingMenu;
    public GameObject gameOverMenu;
    public GameObject gameWonMenu;
    public GameObject playerGameUI;
    public GameObject playerDamageScreen;
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

        mainMenuController = mainMenu.GetComponent<MainMenuController>();
        optionsMenuController = optionsMenu.GetComponent<OptionsMenuController>();
        mainMenuController.setUserInterfaceManager(inst);
        optionsMenuController.setUserInterfaceManager(inst);

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

    public void KeyRebinding()
    {
        EnableMenuElement(keyRebindingMenu);
    }

    public void KeyRebindingBackButton()
    {
        EnableMenuElement(optionsMenu);
    }

    public void GameStart()
    {
        loadingScreen.SetActive(false);
        background.SetActive(false);
        playerGameUI.SetActive(true);
    }

    public void DisplayDamage()
    {
        // canvas.SetActive(true);
    }

    public void HideDamage()
    {
        // canvas.SetActive(false);
    }

    public void updateProgressBar(float progress)
    {
        progressBar.value = progress;
    }

    public void LoadFirstLevel()
    {
        sceneMgr.LoadSceneByName(gameMgr.firstLevelName);
    }

    public void EnableMenuElement(GameObject element)
    {
        canvas.SetActive(true);
        background.SetActive(true);
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
}
