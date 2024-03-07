using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    private void Awake()
    {
        inst = this;
    }

    private SceneManager sceneMgr;
    private QuestManager questMgr;
    private PlayerManager playerMgr;

    public int numLevels;       // number of levels in the game
    public int numLevelsCompleted;      //  number of levels player has completed

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadUserInterfaceScene());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator LoadUserInterfaceScene()
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UserInterfaceScene", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
