using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevel : MonoBehaviour
{
    public LevelData currLevel;
    
    public int numCombatArenas;

    public GameObject arenaPrefab;
    public GameObject questCharPrefab;

    // Not implemented yet: actual procedural path generation
        // instantiate level (grid of arenaPrefabs assiged differing attributes)

    // Start is called before the first frame update
    void Start()
    {
        LoadLevel();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function to initially load 
    void LoadLevel()
    {
        // procedurally generate arena locations


        // for prototype level, for now just generates one combat arena in center of map
        LoadCombatArena();
    }

    void LoadCombatArena()
    {
        GameObject arenaInstance;
        Arena arenaScript;
        arenaInstance = Instantiate(arenaPrefab, gameObject.transform);
        // need to add error checking if instantiated correctly

        arenaScript = arenaInstance.gameObject.GetComponent<Arena>();

        // set arena values (isEmpty, isBossLevel, hasCharacter, arenaLevel, numDoors)
        arenaScript.SetInitialValues(false, false, false, currLevel, 1);

        // if instantiated correctly add to list of arenas in level? 
    }
}
