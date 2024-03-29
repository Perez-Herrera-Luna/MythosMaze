using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager inst;
    public UserInterfaceManager userInterfaceMgr;
    public GameManager gameManager;
    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void setGameManager(GameManager gm)
    {
        gameManager = gm;
    }

    public void setUserInterfaceManager(UserInterfaceManager uim)
    {
        userInterfaceMgr = uim;
    }

    public void LoadPlayerAndUserInterfaceScenes()
    {
        StartCoroutine(LoadUserInterfaceScene());
        // StartCoroutine(LoadPlayerScene());
    }

    public void LoadSceneByName(string sceneName)
    {
        userInterfaceMgr.GameLoading();
        StartCoroutine(LoadScene(sceneName));
    }

    public void LoadPlayerSceneDone()
    {
        // Do something after the player scene is loaded
    }

    public void LoadUserInterfaceSceneDone()
    {
        // Do something after the user interface scene is loaded
    }

    IEnumerator LoadUserInterfaceScene()
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UserInterfaceScene", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {

            yield return null;
        }

        LoadUserInterfaceSceneDone();
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

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        ProceduralLevel currLevel = GameObject.Find("ProceduralLevel").GetComponent<ProceduralLevel>();

        Task levelGeneration = currLevel.GenerateLevelAsync();

        while (!levelGeneration.IsCompleted)
        {
            yield return null;
        }

        currLevel.LoadLevel();
        gameManager.GameStart();

        yield return null;
    }
}
