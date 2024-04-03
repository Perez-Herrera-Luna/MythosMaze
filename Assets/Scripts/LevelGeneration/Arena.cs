using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Arena : MonoBehaviour
{
    private LevelData arenaLevel;      // arena level data (determines max enemy power/arena setup)
    public ArenaData arenaData;
    private bool isBossArena;    // designates if combatArena is boss level or not
    private bool isSourceArena;
    private bool hasCharacter;   // designates if this arena has a quest character within it or not   
    public bool arenaActive = false;    // arena is active when player enters, inactive when player elsewhere in level
    public bool arenaCompleted = false; // arena is completed once player defeats all enemies within

    private List<Vector2Int> activeDoors = new List<Vector2Int>();
    public List<GameObject> doorGameObjects;
    private List<Vector2Int> doorLocations = new List<Vector2Int>();

    private List<GameObject> enemyPrefabs = new List<GameObject>();

    public bool IsSourceArena => isSourceArena;
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

    public void SetInitialValues(bool isSrcArena, LevelData currLevel, List<Vector2Int> availableDoorLocs)
    {
        if (arenaData.isBossArena)
            isBossArena = true;

        if (arenaData.charPrefabNames.Count > 0)
            hasCharacter = true;

        isSourceArena = isSrcArena;
        arenaLevel = currLevel;
        activeDoors = arenaData.doorLocations.Except(availableDoorLocs).ToList();

        doorLocations = isBossArena ? activeDoors : arenaData.doorLocations;

        if (doorLocations.Count != doorGameObjects.Count)
            Debug.Log("Error: mismatch in arena door locations / door game objects");

        /*Debug.Log("arena active doors: ");

        foreach(var door in activeDoors)
        {
            Debug.Log(door);

        }*/

        SetupDoors();

        LoadPrefabs();

        SetupEnemies();
        SetupPowerups();

        if(hasCharacter)
            SetupCharacter();
    }

    private GameObject GetDoor(Vector2Int door)
    {
        int doorIndex = -1;
        for(int i = 0; i < doorLocations.Count; i++)
        {
            if (doorLocations[i] == door)
                doorIndex = i;
        }

        if (doorIndex == -1)
        {
            Debug.Log("error, incorrect door location");
            return null;
        }
        else
        {
            return doorGameObjects[doorIndex];
        }
    }

    private void SetupDoors()
    {
        CloseDoors();

        foreach(Vector2Int door in activeDoors)
        {
            GetDoor(door).SetActive(false);
        }
    }

    private void CloseDoors()
    {
        foreach(GameObject door in doorGameObjects)
        {
            door.SetActive(true);
        }
    }

    private void LoadPrefabs()
    {
        foreach(string enemyName in arenaData.enemyPrefabNames)
        {
            GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemies/" + enemyName);

            if (enemyPrefab == null)
                Debug.Log("Error finding enemy prefab in Assets/Resources/Prefabs/Enemies folder");
            else
                enemyPrefabs.Add(enemyPrefab);
        }
    }

    private void SetupEnemies()
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

    private void SetupPowerups()
    {
        // procedurally generate pickup locations

        // for initial prototype just spawn one type of pickup at set locations
        /*for(int powerupNum = 0; powerupNum < arenaData.maxPowerups; powerupNum++)
        {
            Instantiate(powerupPrefabs[powerupNum], gameObject.transform);
        }*/
    }

    private void SetupCharacter()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            if (!arenaCompleted)
            {
                arenaActive = true;
                // CloseDoors();
            }
        }
    }
}
