using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_PowerUps : MonoBehaviour
{
    public PlayerData playerData;
    public int playerBaseHealth = 10;

    // Start is called before the first frame update
    void Start()
    {
        clearPowerUps();
    }

    // Update is called once per frame
    void Update()
    {
        checkPowerUp();
    }

    void checkPowerUp()
    {
        switch(playerData.powerUpName)
        {
            case "none":
                //nothing
                break;

            case "buffWeaponCooldown":
                //buff code
                break;

            case "restoreHealth":
                playerData.playerHealth = playerData.playerMaxHealth;
                clearPowerUps();
                break;

            case "buffSpeed":
                playerData.moveSpeed *= playerData.powerUpAmount;
                StartCoroutine(speedDuration(playerData.powerUpDuration));
                break;

            case "oneUp":
                playerData.playerMaxHealth += playerBaseHealth;
                break;

        }
    }

    IEnumerator speedDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        playerData.moveSpeed /= playerData.powerUpAmount;
    }

    void clearPowerUps()
    {
        playerData.powerUpAmount = 0;
        playerData.powerUpName = "none";
        playerData.powerUpDuration = 0;
    }
}
