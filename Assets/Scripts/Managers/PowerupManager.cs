using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager inst;

    // private PlayerManager playerMgr;
    // private GameManager gameMgr;

    private string currName;
    private float currAmount;
    private float currDuration;

    private float pickupDelay = 0.2f;
    private bool canPickup = true;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

/*    public void setPlayerManager(PlayerManager player)
    {
        playerMgr = player;
    }*/

    public void ActivatePowerup(string powerupName, float amount)
    {
        if (canPickup)
        {
            currName = powerupName;
            currAmount = amount;
            canPickup = false;
            StartCoroutine(PowerupTimer());
            Debug.Log("Player picked up powerup: " + powerupName);

            switch (currName)
            {
                case "WeaponBuff":
                    PlayerManager.inst.buffWeapons(amount);
                    break;

                case "AbsoluteDefense":
                    PlayerManager.inst.buffAbsoluteDefense(amount);
                    break;

                case "MaxHealth":
                    PlayerManager.inst.buffMaxHealth(amount);
                    break;

                case "Healing":
                    PlayerManager.inst.healPlayer(amount);
                    break;

                case "MoveSpeed":
                    PlayerManager.inst.buffMoveSpeed(amount);
                    break;

                case "DashCooldown":
                    PlayerManager.inst.buffDashCooldown(amount);
                    break;

                case "JumpForce":
                    PlayerManager.inst.buffJumpForce(amount);
                    break;

                case "WeaponDamage":
                    PlayerManager.inst.buffWeaponDamage(amount);
                    break;

                default:
                    Debug.Log("Error: Powerup Name not found in Powerup Manager");
                    break;
            }

            clearPowerUps();
        }
    }

    IEnumerator PowerupTimer()
    {
        yield return new WaitForSeconds(pickupDelay);
        canPickup = true;
    }

    void clearPowerUps()
    {
        currName = "none";
        currAmount = 0;
    }
}
