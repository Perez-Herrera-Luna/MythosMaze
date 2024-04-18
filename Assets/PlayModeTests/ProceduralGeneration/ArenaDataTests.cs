using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ArenaDataTests
{
    private ArenaData[] arenas;

    [SetUp]
    public void Setup()
    {
        arenas = Resources.LoadAll<ArenaData>("DataAssets");
    }

    [UnityTest]
    public IEnumerator ArenaData_CheckForMatchingArenaTypeAndNumDoorLocations()
    {
        bool allMatching = true;

        foreach(ArenaData currArena in arenas)
        {
            int doors = currArena.doorLocations.Count;
            if(doors < 2 | doors > 4){
                Debug.Log("Error: incorect number of doorLocations (not within range 2-4) on arena with dim " + currArena.height + ", " + currArena.width);
                allMatching = false;
            } else if(currArena.isBossArena & doors == 3){
                Debug.Log("Error: incorrect number of doors (3) for boss arena with dim " + currArena.height + ", " + currArena.width);
                allMatching = false;
            }
        }

        Assert.IsTrue(allMatching);

        yield return null;
    }
}
