using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProceduralGenerationTests
{
    // // A Test behaves as an ordinary method
    // [Test]
    // public void ProcGenerTestsSimplePasses()
    // {
    //     // Use the Assert class to test conditions
    // }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ValidatePathToBossArena()
    {
        GameObject level = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/LevelGeneration/ProceduralLevel"));

        ProceduralLevel levelScript = level.GetComponent<ProceduralLevel>();

        // allow time for procedural generation to occur
        yield return new WaitForSeconds(30);

        Assert.AreEqual(1, levelScript.GetLevelGraph.GetBossArena.NumDoors);
    }

    /* [UnityTest]
    public IEnumerator TestInitialArenaSetup()
    {
        GameObject gameArena = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/LevelGeneration/CombatArena"));

        LevelData prototypeLvl = Resources.Load<LevelData>("DataAssets/PrototypeLevel");

        Arena arenaScript = gameArena.GetComponent<Arena>();

        // set initial boss levels as - isBossLevel, hasChar, currLevel, doors
        arenaScript.SetInitialValues(true, false, prototypeLvl, 3);

        yield return null;

        bool arenaSetup = true;

        if(!arenaScript.IsBossLevel | arenaScript.HasCharacter)
            arenaSetup = false;

        if(arenaScript.GetLevel != 0)
            arenaSetup = false;

        if(arenaScript.NumDoors != 3)
            arenaSetup = false;

        Assert.IsTrue(arenaSetup);

        Object.Destroy(gameArena.gameObject);
    } */
}
