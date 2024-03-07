using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.UI;
using UnityEngine;

public class UserInterfaceManager : MonoBehaviour
{
    public GameObject background;
    public GameObject loadingScreen;
    public GameObject optionsMenu;
    public GameObject mainMenu;
    public Slider progressBar;
    public SceneManager sceneMgr;

    // Start is called before the first frame update
    void Start()
    {
        background = GameObject.Find("Background");
        mainMenu = GameObject.Find("MainMenu");
        optionsMenu = GameObject.Find("OptionsMenu");
        loadingScreen = GameObject.Find("LoadingScreen");
        progressBar = GameObject.Find("ProgressBar").GetComponent<Slider>();

        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        loadingScreen.SetActive(false);

        sceneMgr = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        sceneMgr.setLoadScreen(loadingScreen);
        sceneMgr.setBackGround(background);
        sceneMgr.setProgressBar(progressBar);
    }
}
