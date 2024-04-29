using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Arena : MonoBehaviour
{
    [Header("Data Files")]
    private LevelData arenaLevel;      // arena level data (determines max enemy power/arena setup)
    public ArenaData arenaData;

    [Header("Arena Attributes")]
    private bool isBossArena;    // designates if combatArena is boss level or not
    private bool isSourceArena;

    public bool arenaActive = false;    // arena is active when player enters, inactive when player elsewhere in level
    public bool arenaCompleted = false; // arena is completed once player defeats all enemies within

    [Header("Quest Character")]
    private bool hasCharacter;   // designates if this arena has a quest character within it or not   
    public Transform CharacterLoc;

    [Header("Arena Doors")]
    private List<Vector2Int> activeDoors = new List<Vector2Int>();
    public List<GameObject> doorGameObjects;
    private List<Vector2Int> doorLocations = new List<Vector2Int>();

    [Header("Arena Enemies")]
    private List<GameObject> enemyPrefabs = new List<GameObject>();
    private int numEnemies = 0;
    public List<Transform> enemyLocs;

    [Header("Arena Powerups")]
    private List<GameObject> activePowerupPrefabs = new List<GameObject>();
    public List<GameObject> activePowerups = new List<GameObject>();
    public List<Transform> activePowerupLocs;

    private List<GameObject> completePowerupPrefabs = new List<GameObject>();
    private List<GameObject> completePowerups = new List<GameObject>();
    public List<Transform> completePowerupLocs;

    // ********** PUBLIC FUNCTIONS ********** // 
    // public bool ArenaActive => arenaActive;
    // public bool ArenaCompleted => arenaCompleted;
    public bool IsSourceArena => isSourceArena;
    public bool IsBossArena => isBossArena;
    public bool HasCharacter => hasCharacter;
    public int GetLevel => arenaLevel.level;

    // Sets initial arena values (called in ProceduralLevel.cs)
    public void SetInitialValues(bool isSrcArena, LevelData currLevel, List<Vector2Int> availableDoorLocs)
    {
        if (arenaData.isBossArena)
            isBossArena = true;

        if (arenaData.charPrefabName != null)
            hasCharacter = true;

        isSourceArena = isSrcArena;
        arenaLevel = currLevel;
        activeDoors = arenaData.doorLocations.Except(availableDoorLocs).ToList();

        doorLocations = isBossArena ? activeDoors : arenaData.doorLocations;

        if (doorLocations.Count != doorGameObjects.Count)
            Debug.Log("Error: mismatch in arena door locations / door game objects");

        StartCoroutine(SetupArena());
    }

    public bool AddPowerup(PowerupData powerup, bool firstRound)
    {
        // 2 rounds of powerup generation (round 2 is if there are less than min number of powerups in level)
        if (powerup.generationLocation == "arena_active")
        {
            if (firstRound)
                return AddActivePowerup(powerup.powerupName, powerup.generationProbability);
            else
                return AddActivePowerup(powerup.powerupName, 1);
        }else if (powerup.generationLocation == "arena_complete")
        {
            if (!hasCharacter)
            {
                if (firstRound)
                    return AddCompletePowerup(powerup.powerupName, powerup.generationProbability);
                else
                    return AddCompletePowerup(powerup.powerupName, 1);
            }
            else
            {
                Debug.Log("warning: trying to add complete powerup to src arena");
                return false;
            }
        }
        else
        {
            Debug.Log("warning: trying to add incorrect powerup type to arena");
            return false;
        }
    }

    // Sets up arena powerups, returns a list corresponding to number of active arena prefabs implemented of each type
    private bool AddActivePowerup(string powerupName, float powerupProb)
    {
        if (activePowerupLocs.Count > 0) {

            List<Transform> powerupLocs = activePowerupLocs;

            foreach (Transform loc in powerupLocs)
            {
                float probability = ThreadSafeRandom.GetRandomProb();
                if(probability <= powerupProb)
                {
                    GameObject currPowerup = Resources.Load<GameObject>("Prefabs/Powerups/" + powerupName);
                    activePowerups.Add(Instantiate(currPowerup, loc));
                    activePowerupLocs.Remove(loc);
                    return true;
                }
                else
                {
                    Debug.Log("probability: " + probability);
                }
            }

            Debug.Log("warning: powerup " + powerupName + " did not activate in arena");
            return false;
        }else{
            // Debug.Log("warning: no more active powerup locations");
            return false;
        }
    }

    private bool AddCompletePowerup(string powerupName, float powerupProb)
    {
        if (completePowerupLocs.Count >= 0)
        {
            List<Transform> powerupLocs = completePowerupLocs;

            foreach (Transform loc in powerupLocs)
            {
                float probability = ThreadSafeRandom.GetRandomProb();
                if (probability <= powerupProb)
                {
                    GameObject currPowerup = Resources.Load<GameObject>("Prefabs/Powerups/" + powerupName);
                    completePowerups.Add(Instantiate(currPowerup, loc));
                    completePowerupLocs.Remove(loc);
                    return true;
                }
            }

            Debug.Log("warning: powerup " + powerupName + " did not activate in arena");
            return false;
        }
        else
        {
            Debug.Log("warning: no more complete powerup locations");
            return false;
        }
    }

    // Called by all arena enemies (in Enemy.cs) on EnemyDeath
    public void EnemyDeath()
    {
        numEnemies--;
        if (numEnemies == 0)
        {
            arenaCompleted = true;
            StartCoroutine(CompleteArena());
        }
        else if (numEnemies < 0)
        {
            Debug.Log("Called arena.EnemyDeath() more times than needed for arena.numEnemies");
        }
    }


    // ********** COLLISION DETECTION ********** // 

    // Detects when player enters current arena
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            StartCoroutine(ActivateArena());
        }
        else
        {
            // Debug.Log("warning: Arena.OnTriggerEnter called on non-player obj");
        }
    }

    // Detects when player exits current arena
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "player")
        {
            arenaActive = false;
            StartCoroutine(DeactivateArena());
        }
        else
        {
            // Debug.Log("warning: Arena.OnTriggerExit called on non-player obj");
        }
    }


    // ********** COROUTINES ********** // 

    // Coroutine: Loads Arena Prefabs, Setups up enemies, powerups, and character
    IEnumerator SetupArena()
    {
        LoadPrefabs();

        SetupEnemies();
        // SetupPowerups();

        if(!isSourceArena)
            DeactivatePowerups();

        if (hasCharacter)
            SetupCharacter();

        yield return new WaitForSeconds(0.5f);

        // TODO : Add final checks for arena 
    }

    // Coroutine: On player entry, closes doors and activates powerups as appropriate
    IEnumerator ActivateArena()
    {
        yield return null;

        if (!arenaActive)
        {
            if (!arenaCompleted)
                CloseDoors();

            ActivatePowerups();
            Debug.Log("arena activated");
            arenaActive = true;
        }
        /*else
        {
            Debug.Log("warning: arena bool already active");
        }*/
    }

    // Coroutine: On player exit, deactivates powerups
    IEnumerator DeactivateArena()
    {
        yield return new WaitForSeconds(5.0f);

        if (!arenaActive)
        {
            DeactivatePowerups();
            arenaActive = false;
        }
        else
        {
            Debug.Log("Warning: Trying to deactivate arena with player in it");
        }
    }

    // Coroutine: if hasCharacter then starts character's scripts else activates powerups and opens doors
    IEnumerator CompleteArena()
    {
        yield return null;

        if (arenaCompleted)
        {
            OpenDoors();

            if (!hasCharacter)
            {
                ActivatePowerups();
            }
            else
            {
                ActivateCharacter();
            }
        }
        else
        {
            Debug.Log("Error: Called CompleteArena on incomplete arena");
        }
    }

    // ********** INITIAL SETUP ********** // 

    // helper function: loads needed enemy (and maybe other - powerup, char) prefabs from Resources folder
    private void LoadPrefabs()
    {
        foreach (string enemyName in arenaData.enemyPrefabNames)
        {
            // TODO : compile prefabs into one folder

            GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemies/" + enemyName);
            if (enemyPrefab == null)
                enemyPrefab = Resources.Load<GameObject>("ImportedAssets/Original_Models/Enemies/Skeleton/Skeleton_Enemy");

            if (enemyPrefab == null)
                Debug.Log("Error finding enemy prefab in Assets/Resources/Prefabs/Enemies folder");
            else
                enemyPrefabs.Add(enemyPrefab);
        }

        /*foreach (string powerupName in arenaData.activePowerupNames)
        {
            // TODO : compile prefabs into one folder

            GameObject activePowerup = Resources.Load<GameObject>("Prefabs/Powerups/" + powerupName);
            
            if (activePowerup == null)
                Debug.Log("Error finding powerup prefab in Assets/Resources/Prefabs/Powerups folder");
            else
                activePowerupPrefabs.Add(activePowerup);
        }*/
    }

    // helper function: Instantiates enemies at procedurally generated locations
    private void SetupEnemies()
    {
        // TODO: procedurally generate enemy locations

        // for now, just manually setting enemy locations using unity interface

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

        // TODO : add test if enemy prefabs were found

        // for initial prototype just spawn one type of enemy at set locations
        for (int enemyNum = 0; enemyNum < maxEnemies; enemyNum++)
        {
            Instantiate(enemyPrefabs[0], enemyLocations[enemyNum], rotation, gameObject.transform);
            numEnemies++;
        }
    }

    // helper function: generates a random, valid enemy location
    /*private Vector3 GenerateEnemyLocation()
    {
        return Vector3.zero;
    } */

    // helper function: instantiates all arena powerups at procedurally generated locations
    /*private void SetupPowerups()
    {
        // TODO: procedurally generate pickup locations

        // setup arena active powerups
        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        List<Vector3> activePowerupLocs = new List<Vector3>();
        Vector3 loc = new Vector3(2, 0.85f, 2);
        activePowerupLocs.Add(loc + gameObject.transform.position);
        loc = new Vector3(-8, 0.85f, 20);
        activePowerupLocs.Add(loc + gameObject.transform.position);
        loc = new Vector3(-20, 0.85f, 10);
        activePowerupLocs.Add(loc + gameObject.transform.position);

        // for initial prototype just spawn one type of pickup at set locations
        for (int powerupNum = 0; powerupNum < arenaData.maxNumPowerups; powerupNum++)
        {
            activePowerups.Add(Instantiate(activePowerupPrefabs[powerupNum], activePowerupLocs[powerupNum], rotation, gameObject.transform));
        }

        // setup arena completion powerup(s)
        Quaternion rotation2 = Quaternion.Euler(4.047f, 12.59f, -88.822f);

        List<Vector3> completePowerupLocs = new List<Vector3>();
        Vector3 powerupLoc = new Vector3(0, 0.85f, 0);
        completePowerupLocs.Add(powerupLoc + gameObject.transform.position);

        if(completePowerupLocs.Count != completePowerupPrefabs.Count)
        {
            Debug.Log("error: incorrect number of powerup prefabs");
            return;
        }

        for (int powerupNum = 0; powerupNum < completePowerupLocs.Count; powerupNum++)
        {
            completePowerups.Add(Instantiate(completePowerupPrefabs[powerupNum], completePowerupLocs[powerupNum], rotation, gameObject.transform));
        }

        DeactivatePowerups();
    }*/

    // helper function: generates a random, valid powerup location
    /*private Vector3 GeneratePowerupLocation()
    {
        return Vector3.zero;
    } */

    // helper function: instantiates (and deactivates) character in center of arena
    private void SetupCharacter()
    {
        // TODO : this (setupcharacter)
    }


    // ********** POWERUPS ********** // 

    // helper function: activates appropriate powerups (active | completed) based on arena state
    private void ActivatePowerups()
    {
        UpdatePowerups();
        if (arenaActive)
        {
            if (arenaCompleted)
            {
                foreach (GameObject powerup in completePowerups)
                {
                    powerup.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject powerup in activePowerups)
                {
                    powerup.SetActive(true);
                }
            }
        }
    }

    // helper function: deactivates all arena powerups
    private void DeactivatePowerups()
    {
        UpdatePowerups();
        foreach (GameObject powerup in activePowerups)
        {
            powerup.SetActive(false);
        }

        foreach (GameObject powerup in completePowerups)
        {
            powerup.SetActive(false);
        }
    }

    // helper function: check if powerups have been deactivated
    private void UpdatePowerups()
    {
        List<GameObject> powerupsToRemove = new List<GameObject>();
        foreach (GameObject powerup in activePowerups)
        {
            if (powerup == null)
                powerupsToRemove.Add(powerup);
        }

        foreach(GameObject removePowerup in powerupsToRemove)
        {
            activePowerups.Remove(removePowerup);
        }
    }

    // ********** QUEST CHARACTER ********** // 

    // helper function: activates quest character
    private void ActivateCharacter()
    {
    }

    // ********** DOORS ********** // 

    // helper function: returns the arena door GameObject at the provided location
    private GameObject GetDoor(Vector2Int door)
    {
        int doorIndex = -1;
        for (int i = 0; i < doorLocations.Count; i++)
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

    // helper function: opens active doors in an arena
    private void OpenDoors()
    {
        CloseDoors();

        foreach (Vector2Int door in activeDoors)
        {
            GetDoor(door).SetActive(false);
        }
    }

    // helper function: close all arena doors
    private void CloseDoors()
    {
        foreach (GameObject door in doorGameObjects)
        {
            door.SetActive(true);
        }
    }
}
