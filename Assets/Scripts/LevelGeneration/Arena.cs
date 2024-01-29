using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public bool isEmpty;        // designates arena as either empty (path) or non-empty (combatArena)
    public bool isBossLevel;    // designates if combatArena is boss level or not
    public bool hasCharacter;   // designates if this arena has a quest character within it or not    
    public LevelData arenaLevel;      // arena level data (determines max enemy power/arena setup)
    public int numDoors;

    public bool arenaActive = false;    // arena is active when player enters, inactive when player elsewhere in level

    public List<GameObject> enemyPrefabs;    // list of monster prefabs that can appear in this level
    public List<GameObject> powerupPrefabs;   // list of enemyPrefabs that can appear in this level
    GameObject charPrefab;       // quest character prefab
    GameObject itemPrefab;       // quest item prefab

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInitialValues(bool isPath, bool bossLevel, bool hasChar, LevelData currLevel, int doors)
    {
        isEmpty = isPath;
        isBossLevel = bossLevel;
        hasCharacter = hasChar;
        arenaLevel = currLevel;
        numDoors = doors;

        SetupDoors();

        if(!isEmpty)
        {
            SetupEnemies();
            SetupPowerups();
        }

        if(hasCharacter)
            SetupCharacter();

    }

    public void SetupDoors()
    {

    }

    public void SetupEnemies()
    {
        // procedurally generate enemy locations

        // for initial prototype just spawn one type of enemy at set locations
        for(int enemyNum = 0; enemyNum < arenaLevel.maxEnemies; enemyNum++)
        {
            // Instantiate(enemyPrefabs[0], gameObject.transform);
        }
    }

    public void SetupPowerups()
    {
        // procedurally generate pickup locations

        // for initial prototype just spawn one type of pickup at set locations
        for(int powerupNum = 0; powerupNum < arenaLevel.maxPowerups; powerupNum++)
        {
            Instantiate(powerupPrefabs[powerupNum], gameObject.transform);
        }
    }

    public void SetupCharacter()
    {

    }
}
