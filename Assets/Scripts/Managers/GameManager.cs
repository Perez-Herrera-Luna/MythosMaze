using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    private void Awake(){
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
}
