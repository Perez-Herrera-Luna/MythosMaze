using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public bool isEmpty;        // designates arena as either empty (path) or non-empty (combatArena)
    public bool isBossLevel;    // designates if combatArena is boss level or not
    public bool hasCharacter;   // designates if this arena has a quest character within it or not
    public bool arenaActive;    // arena is active when player enters, inactive when player elsewhere in level
    
    public LevelData arenaLevel;      // arena level data (determines max enemy power/arena setup)
    public int numEnemies;
    public int numPowerups;
    public int numDoors;

    public List<GameObject> enemyPrefabs;    // list of monster prefabs that can appear in this level
    public List<GameObject> powerupPrefabs;   // list of enemyPrefabs that can appear in this level

    public GameObject charPrefab;       // quest character prefab
    public GameObject itemPrefab;       // quest item prefab

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
