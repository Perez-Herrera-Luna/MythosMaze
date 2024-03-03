using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    private LevelData arenaLevel;      // arena level data (determines max enemy power/arena setup)
    public ArenaData arenaData;
    private bool isBossLevel;    // designates if combatArena is boss level or not
    private bool hasCharacter;   // designates if this arena has a quest character within it or not   
    private int numDoors;
    public bool arenaActive = false;    // arena is active when player enters, inactive when player elsewhere in level

    public List<GameObject> enemyPrefabs;    // list of monster prefabs that can appear in this level
    public List<GameObject> powerupPrefabs;   // list of enemyPrefabs that can appear in this level
    
    public List<GameObject> bossPrefabs;      // boss
    public List<GameObject> charPrefabs;       // quest character prefab
    public List<GameObject> itemPrefabs;       // quest item prefab

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInitialValues(bool bossLevel, bool hasChar, LevelData currLevel, int doors)
    {
        isBossLevel = bossLevel;
        hasCharacter = hasChar;
        arenaLevel = currLevel;
        numDoors = doors;

        SetupDoors();

        SetupEnemies();
        SetupPowerups();

        if(hasCharacter)
            SetupCharacter();

    }

    public void SetupDoors()
    {
        // curr number of doors = (4 - availableDoors.Count)
    }

    public void SetupEnemies()
    {
        // procedurally generate enemy locations

        // for initial prototype just spawn one type of enemy at set locations
        /*for(int enemyNum = 0; enemyNum < arenaData.maxEnemies; enemyNum++)
        {
            // Instantiate(enemyPrefabs[0], gameObject.transform);
        }*/
    }

    private Vector2 GenerateEnemyLocation()
    {
        return Vector2.zero;
    } 
    public void SetupPowerups()
    {
        // procedurally generate pickup locations

        // for initial prototype just spawn one type of pickup at set locations
        /*for(int powerupNum = 0; powerupNum < arenaData.maxPowerups; powerupNum++)
        {
            Instantiate(powerupPrefabs[powerupNum], gameObject.transform);
        }*/
    }

    public void SetupCharacter()
    {

    }
}
