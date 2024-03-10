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

    public GameObject canvas;
    public GameObject background;
    public GameObject loadingScreen;
    public GameObject optionsMenu;
    public GameObject mainMenu;
    public GameObject gameOverMenu;
    public GameObject gameWonMenu;
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
        optionsMenu = GameObject.Find("OptionsMenu");
        loadingScreen = GameObject.Find("LoadingScreen");
        gameOverMenu = GameObject.Find("GameOverMenu");
        gameWonMenu = GameObject.Find("GameWinMenu");
        playerDamageScreen = GameObject.Find("PlayerDamageScreen");
        progressBar = GameObject.Find("ProgressBar").GetComponent<Slider>();

        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        loadingScreen.SetActive(false);
        gameOverMenu.SetActive(false);
        gameWonMenu.SetActive(false);
        playerDamageScreen.SetActive(false);

        sceneMgr = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
        sceneMgr.setUserInterfaceManager(inst);
        gameMgr.setUserInterfaceManager(inst);
    }

    public void GameLoading()
    {
        canvas.SetActive(true);
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        background.SetActive(true);
    }

    public void GameWon()
    {
        canvas.SetActive(true);
        mainMenu.SetActive(false);
        gameWonMenu.SetActive(true);
        background.SetActive(true);
    }

    public void GameOver()
    {
        canvas.SetActive(true);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        background.SetActive(true);
    }

    public void GameStart()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        loadingScreen.SetActive(false);
        background.SetActive(false);
        canvas.SetActive(false);
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
}
