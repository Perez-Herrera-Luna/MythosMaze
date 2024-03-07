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
                loadingScreen.SetActive(false);
                background.SetActive(false);
            }

            yield return null;
        }
    }
}
