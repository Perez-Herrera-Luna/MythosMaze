using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyAITesting
{
    // A Test behaves as an ordinary method
    // [Test]
    // public void EnemyAITestingSimplePasses()
    // {
    //     // Use the Assert class to test conditions
    // }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator EnemyAITestingDefaultEnemyType()
    {        
        GameObject testEnemy = Resources.Load<GameObject>("Prefabs/EnemyPrefab");

        Assert.IsNotNull(testEnemy, "Prefab not loaded");

        Monster monsterScript = testEnemy.GetComponent<Monster>();

        Assert.IsNotNull(monsterScript, "Monster component not found");

        yield return null;

        Assert.AreEqual("Basic_Melee", monsterScript.enemyType, "Enemy type not set correctly");
    }
}
