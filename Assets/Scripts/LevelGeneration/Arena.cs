using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Arena : MonoBehaviour
{
    private LevelData arenaLevel;      // arena level data (determines max enemy power/arena setup)
    public ArenaData arenaData;
    private bool isBossArena;    // designates if combatArena is boss level or not
    private bool hasCharacter;   // designates if this arena has a quest character within it or not   
    public bool arenaActive = false;    // arena is active when player enters, inactive when player elsewhere in level

    public List<GameObject> enemyPrefabs;    // list of monster prefabs that can appear in this level
    public List<GameObject> powerupPrefabs;   // list of enemyPrefabs that can appear in this level
    
    public List<GameObject> bossPrefabs;      // boss
    public List<GameObject> charPrefabs;       // quest character prefab
    public List<GameObject> itemPrefabs;       // quest item prefab

    private List<Vector2Int> activeDoors = new List<Vector2Int>();
    public List<GameObject> doorGameObjects;

    public bool IsBossArena => isBossArena;
    public bool HasCharacter => hasCharacter;
    public int GetLevel => arenaLevel.level;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInitialValues(bool bossArena, bool hasChar, LevelData currLevel, List<Vector2Int> availableDoorLocs)
    {
        isBossArena = bossArena;
        hasCharacter = hasChar;
        arenaLevel = currLevel;
        activeDoors = arenaData.doorLocations.Except(availableDoorLocs).ToList();

        SetupDoors();

        SetupEnemies();
        SetupPowerups();

        if(hasCharacter)
            SetupCharacter();
    }

    public void SetupDoors()
    {
        foreach(Vector2Int door in activeDoors)
        {
            // north/south doors
            if(door.x == 0)
            {
                if (door.y > 0)
                    doorGameObjects[0].SetActive(false);
                else
                    doorGameObjects[1].SetActive(false);

            }else if(door.y == 0)   // East/West Doors
            {
                if (door.x > 0)
                    doorGameObjects[2].SetActive(false);
                else
                    doorGameObjects[3].SetActive(false);
            }
        }
    }

    public void SetupEnemies()
    {
        // procedurally generate enemy locations

        // for demo, just manually setting enemy locations using unity interface

        int maxEnemies = 0;
        if (isBossArena)
            maxEnemies = 1;
        else
            maxEnemies = 3;

        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        List<Vector3> enemyLocations = new List<Vector3>();
        Vector3 loc = new Vector3(0, 1.4f, 0);
        enemyLocations.Add(loc + gameObject.transform.position);
        loc = new Vector3(20, 1.4f, 20);
        enemyLocations.Add(loc + gameObject.transform.position);
        loc = new Vector3(-10, 1.4f, 10);
        enemyLocations.Add(loc + gameObject.transform.position);

        // for initial prototype just spawn one type of enemy at set locations
        for (int enemyNum = 0; enemyNum < maxEnemies; enemyNum++)
        {
            Instantiate(enemyPrefabs[0], enemyLocations[enemyNum], rotation, gameObject.transform);
        }
    }

    /*private Vector2 GenerateEnemyLocation()
    {
        return Vector2.zero;
    } */
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
