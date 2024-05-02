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

    private bool arenaActive = false;    // arena is active when player enters, inactive when player elsewhere in level
    private bool arenaCompleted = false; // arena is completed once player defeats all enemies within

    public bool ArenaActive => arenaActive;

    private bool hasCharacter = false;   // designates if this arena has a quest character within it or not   
    public Transform characterLoc;
    private GameObject characterPrefab;
    private GameObject questCharacter;

    private List<Vector2Int> activeDoors = new List<Vector2Int>();
    public List<GameObject> doorGameObjects;
    private List<Vector2Int> doorLocations = new List<Vector2Int>();

    [Header("Level Generation Locations")]
    public List<Transform> enemyLocs;
    private List<GameObject> enemyPrefabs = new List<GameObject>();
    private List<GameObject> enemyPowerupPrefabs = new List<GameObject>();
    private int numEnemies = 0;
    private int maxTriesGenEnemy = 10;

    private List<GameObject> activePowerupPrefabs = new List<GameObject>();
    private List<GameObject> activePowerups = new List<GameObject>();
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

        if (isSrcArena && characterLoc != null)
            hasCharacter = true;

        isSourceArena = isSrcArena;
        arenaLevel = currLevel;
        activeDoors = arenaData.doorLocations.Except(availableDoorLocs).ToList();
        CloseDoors();

        doorLocations = isBossArena ? activeDoors : arenaData.doorLocations;

        if (doorLocations.Count != doorGameObjects.Count)
            Debug.Log("Error: mismatch in arena door locations / door game objects");

        if (isSrcArena)
            SetupSourceArena();
        else
            StartCoroutine(SetupArena());

        OpenDoors();
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
        }else
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
                    Debug.Log("Added powerup: " + powerupName + " to arena");
                    return true;
                }
                /*else
                {
                    Debug.Log("probability: " + probability + " / " + powerupProb);
                }*/
            }

            // Debug.Log("warning: powerup " + powerupName + " did not activate in arena");
            return false;
        }else{
            // Debug.Log("warning: no more active powerup locations");
            return false;
        }
    }

    private bool AddCompletePowerup(string powerupName, float powerupProb)
    {
        if (completePowerupLocs.Count > 0)
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
                else
                {
                    Debug.Log("probability: " + probability);
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
    public void EnemyDeath(bool hasPowerup, Vector3 enemyLoc, int powerupIndex)
    {
        if (hasPowerup && powerupIndex != -1)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            Instantiate(enemyPowerupPrefabs[powerupIndex], enemyLoc, rotation, gameObject.transform);
        }

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
            if (!hasCharacter)
            {
                ActivatePowerups();
                OpenDoors();
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

    private void SetupSourceArena()
    {
        LoadPrefabs();
        SetupEnemies();

        if(hasCharacter)
            SetupCharacter();
    }

    // helper function: loads needed enemy (and maybe other - powerup, char) prefabs from Resources folder
    private void LoadPrefabs()
    {
        foreach (string enemyName in arenaData.enemyPrefabNames)
        {
            GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemies/" + enemyName);

            if (enemyPrefab == null)
                Debug.Log("Error finding enemy prefab in Assets/Resources/Prefabs/Enemies folder");
            else
                enemyPrefabs.Add(enemyPrefab);
        }

        foreach (string powerupName in arenaLevel.powerups)
        {
            GameObject currPowerup = Resources.Load<GameObject>("Prefabs/Powerups/" + powerupName);
            if (currPowerup.GetComponent<Powerup>().powerupData.generationLocation == "enemy_dropped")
            {
                Debug.Log("trying to add enemy powerup");
                enemyPowerupPrefabs.Add(currPowerup);
            }
        }

        if (hasCharacter)
        {
            GameObject charPrefab = Resources.Load<GameObject>("Prefabs/Quests/" + arenaData.charPrefabName);
            if (charPrefab == null)
                Debug.Log("Error finding quest character prefab in Assets/Resources/Prefabs/Quests folder");
            else
                characterPrefab = charPrefab;
        }
    }

    // helper function: Instantiates enemies at procedurally generated locations
    private void SetupEnemies()
    {
        Dictionary<GameObject, int> enemiesCount = new Dictionary<GameObject, int>();
        List<GameObject> enemies = new List<GameObject>();

        bool allEnemiesLoaded = false;
        int numTries = 0;
        while ((enemyLocs.Count > 0) & !allEnemiesLoaded && numTries++ < maxTriesGenEnemy)
        {
            foreach (GameObject currEnemy in enemyPrefabs)
            {
                if (!enemiesCount.ContainsKey(currEnemy))
                    enemiesCount.Add(currEnemy, 0);

                EnemyData currEnemyData = currEnemy.GetComponent<Enemy>().enemy;

                if (enemiesCount[currEnemy] < currEnemyData.maxNumPerArena)
                {
                    int randLoc = ThreadSafeRandom.GetRandom(0, enemyLocs.Count);

                    GameObject newEnemy = Instantiate(currEnemy, enemyLocs[randLoc]);
                    if(newEnemy != null)
                    {
                        bool hasPowerup = false;
                        int powerupIndex = -1;

                        Debug.Log("currEnemy: " + currEnemyData.name + " w/ num: " + enemiesCount[currEnemy]);

                        for (int i = 0; i < enemyPowerupPrefabs.Count; i++)
                        {
                            Debug.Log("trying to add powerup to enemy");
                            float powerupProb = enemyPowerupPrefabs[i].GetComponent<Powerup>().powerupData.generationProbability;
                            float randProb = ThreadSafeRandom.GetRandomProb();
                            Debug.Log(powerupProb + " / " + randProb);
                            if (randProb <= powerupProb)
                            {
                                Debug.Log("enemy has powerup");
                                hasPowerup = true;
                                powerupIndex = i;
                            }
                        }

                        enemies.Add(newEnemy);
                        newEnemy.GetComponent<Enemy>().SetArenaAndPowerup(this, hasPowerup, powerupIndex);
                        enemyLocs.RemoveAt(randLoc);
                        enemiesCount[currEnemy] += 1;
                        numEnemies++;
                    }
                }
                else
                {
                    Debug.Log("Error: reached max number of enemies");
                }
            }

            allEnemiesLoaded = true;

            foreach (KeyValuePair<GameObject, int> currEnemy in enemiesCount)
            {
                EnemyData currEnemyData = currEnemy.Key.GetComponent<Enemy>().enemy;
                if (currEnemy.Value < currEnemyData.minNumPerArena)
                    allEnemiesLoaded = false;
            }
        }

        if(enemyLocs.Count == 0)
        {
            Debug.Log("No more enemy locs");
        }else if(allEnemiesLoaded)
        {
            Debug.Log("all enemies loaded");
        }
    }

    // helper function: instantiates (and deactivates) character in center of arena
    private void SetupCharacter()
    {
        if (hasCharacter)
        {
            if(characterPrefab != null)
            {
                questCharacter = Instantiate(characterPrefab, characterLoc);
                questCharacter.GetComponent<QuestCharacter>().SetArenaConnection(this);
                questCharacter.SetActive(false);
            }
        }
    }


    // ********** POWERUPS ********** // 

    // helper function: activates appropriate powerups (active | completed) based on arena state
    private void ActivatePowerups()
    {
        DeactivatePowerups();
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

        foreach (GameObject powerup in completePowerups)
        {
            if (powerup == null)
                powerupsToRemove.Add(powerup);
        }

        foreach (GameObject removePowerup in powerupsToRemove)
        {
            completePowerups.Remove(removePowerup);
        }
    }

    // ********** QUEST CHARACTER ********** // 

    // helper function: activates quest character
    private void ActivateCharacter()
    {
        questCharacter.SetActive(true);
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
    public void OpenDoors()
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
