using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerControllerTest
{
    // // A Test behaves as an ordinary method
    // [Test]
    // public void PlayerControllerTestSimplePasses()
    // {
    //     // Use the Assert class to test conditions
    // }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestPlayerInitialSetup()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/PlayerHolder"));

        GameObject player = GameObject.Find("Player");

        PlayerMovement playerScript = player.GetComponent<PlayerMovement>();

        yield return null;

        bool playerInit = true;

        if (playerScript == null)
        {
            Debug.Log("PlayerMovement script not found");
            Assert.Fail();
        }

        if (playerScript.walkSpeed != 10f)
            playerInit = false;

        if (playerScript.dashSpeed != 18f)
            playerInit = false;

        if (playerScript.jumpForce != 7f)
            playerInit = false;

        if (playerScript.crouchSpeed != 3.5f)
            playerInit = false;

        Assert.IsTrue(playerInit);

        Object.Destroy(GameObject.Find("PlayerHolder"));
    }
}
