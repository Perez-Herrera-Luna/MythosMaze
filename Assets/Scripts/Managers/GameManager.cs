using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public UserInterfaceManager userInterfaceMgr;
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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setUserInterfaceManager(UserInterfaceManager uim)
    {
        userInterfaceMgr = uim;
    }

    public void DisplayDamage()
    {
        userInterfaceMgr.DisplayDamage();
    }

    public void HideDamage()
    {
        userInterfaceMgr.HideDamage();
    }

    public void GameOver()
    {
        userInterfaceMgr.GameOver();
    }

    public void GameWon()
    {
        userInterfaceMgr.GameWon();
    }
}
