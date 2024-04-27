using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager inst;

    private PlayerManager playerMgr;
    private GameManager gameMgr;

    private string currName;
    private float currAmount;
    private float currDuration;

    private void Awake()
    {
        inst = this;
    }

    void Start()
    {
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void setPlayerManager(PlayerManager player)
    {
        playerMgr = player;
    }

    public void ActivatePowerup(string powerupName, float amount)
    {
        currName = powerupName;
        currAmount = amount;

        switch (currName)
        {
            case "weaponBuff":
                playerMgr.buffWeapons(amount);
                break;

            case "maxHealth":
                playerMgr.buffMaxHealth(amount);
                break;

            case "healing":

                break;

            case "moveSpeed":
                break;

            case "dashCooldown":
                break;

            case "jumpForce":
                break;

            default:
                Debug.Log("Error: Powerup Name not found in Powerup Manager");
                break;

        }

        clearPowerUps();
    }

    void clearPowerUps()
    {
        currName = "none";
        currAmount = 0;
    }
}
