using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneManager : MonoBehaviour
{
    public static SceneManager inst;
    public Camera userInterfaceCamera;
    public GameObject loadingScreen;
    public GameObject background;
    // public Slider progressBar;
    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadUserInterfaceScene()); // Load the user interface scene
    }

    public void setLoadScreen(GameObject loadScreen)
    {
        loadingScreen = loadScreen;
    }

    public void setBackGround(GameObject bg)
    {
        background = bg;
    }

    public void LoadSceneByName(string sceneName)
    {
        // progressBar = loadingScreen.GetComponentInChildren<Slider>();
        StartCoroutine(LoadScene(sceneName));
        loadingScreen.SetActive(true);
    }

    public void MainMenuLoadDone()
    {
        userInterfaceCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    IEnumerator LoadUserInterfaceScene()
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UserInterfaceScene", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {

            yield return null;
        }

        MainMenuLoadDone();
    }

    private AsyncOperation asyncLoad;
    IEnumerator LoadScene(string sceneName)
    {
        yield return null;

        asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;


        while (!asyncLoad.isDone)
        {
            float progress = asyncLoad.progress * 100;
            // progressBar.value = progress;

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
                loadingScreen.SetActive(false);
                background.SetActive(false);
                userInterfaceCamera.enabled = false;
            }

            yield return null;
        }
    }
}
