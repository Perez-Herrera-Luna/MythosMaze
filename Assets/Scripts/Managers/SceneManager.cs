using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager inst;
    public GameObject loadingScreen;
    public GameObject background;
    public GameObject canvas;
    public GameObject gameOverMenu;
    public GameObject gameWonMenu;
    public UnityEngine.UI.Slider progressBar;
    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadUserInterfaceScene()); // Load the user interface scene
        StartCoroutine(LoadPlayerScene()); // Load the player scene
    }

    public void setCanvas(GameObject canv)
    {
        canvas = canv;
    }

    public void setGameOverMenu(GameObject gameOver)
    {
        gameOverMenu = gameOver;
    }

    public void setGameWonMenu(GameObject gameWon)
    {
        gameWonMenu = gameWon;
    }

    public void setLoadScreen(GameObject loadScreen)
    {
        loadingScreen = loadScreen;
    }

    public void setBackGround(GameObject bg)
    {
        background = bg;
    }

    public void setProgressBar(UnityEngine.UI.Slider bar)
    {
        progressBar = bar;
    }

    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
        loadingScreen.SetActive(true);
    }

    public void LoadPlayerSceneDone()
    {
        // Do something after the player scene is loaded
    }

    IEnumerator LoadUserInterfaceScene()
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UserInterfaceScene", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {

            yield return null;
        }
    }

    IEnumerator LoadPlayerScene()
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("PlayerScene", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {

            yield return null;
        }

        LoadPlayerSceneDone();
    }

    private AsyncOperation asyncLoad;
    IEnumerator LoadScene(string sceneName)
    {
        yield return null;

        asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;


        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.value = progress;

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;

                GameStart();
            }

            yield return null;
        }
    }

    public void GameStart()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        loadingScreen.SetActive(false);
        background.SetActive(false);
        canvas.SetActive(false);
    }

    public void GameOver()
    {
        canvas.SetActive(true);
        gameOverMenu.SetActive(true);
    }

    public void GameWon()
    {
        canvas.SetActive(true);
        gameWonMenu.SetActive(true);
    }
}
