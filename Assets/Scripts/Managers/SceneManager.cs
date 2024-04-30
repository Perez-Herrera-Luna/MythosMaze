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
    // public GameManager gameManager;
    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

/*    public void setGameManager(GameManager gm)
    {
        gameManager = gm;
    }*/

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

    public void UnloadCurrLevel()
    {
        // TODO : make this actually unload whatever curr level is
        Debug.Log("unload level");
        StartCoroutine(UnloadScene("Level1"));
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

    IEnumerator UnloadScene(string sceneName)
    {
        AsyncOperation asyncUnload = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);

        yield return null;
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

        if (currLevel.Success)
        {
            currLevel.LoadLevel();
            GameManager.inst.GameStart();
        }
        else
        {
            // TODO: 
            // error handling in absolute worst case (aka despite all testing level generation failed
            // should return to main menu
            // maybe popup or smtg saying smtg along the lines of 'error ocurred, sorry, please retry loading game'
        }

        yield return null;
    }
}
