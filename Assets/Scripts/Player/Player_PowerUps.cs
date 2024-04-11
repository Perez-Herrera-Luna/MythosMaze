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
}
