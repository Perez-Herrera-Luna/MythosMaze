using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager inst;

    private GameManager gameMgr;
    private PowerupManager powerupMgr;

    private PlayerMovement playerMov;
    private PlayerWeaponController playerWeapons;

    public bool IsAttacking => playerMov.isAttacking;
    public int ActiveWeapon => playerMov.weaponSelected;
    public int WeaponDamage => playerWeapons.weaponDamage[ActiveWeapon - 1];

    private float maxHealth = 20;
    private float health;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
        powerupMgr = GameObject.Find("PowerupManager").GetComponent<PowerupManager>();
        playerMov = GameObject.Find("Player").GetComponent<PlayerMovement>();
        playerWeapons = GameObject.Find("Player Collider").GetComponent<PlayerWeaponController>();

        powerupMgr.setPlayerManager(inst);
        health = maxHealth;
    }

    private void checkHealth()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        else if (health <= 0)
        {
            health = 0;
            gameMgr.GameOver();
            Debug.Log("Player Died");
        }
    }

    public void playerHit(float damage)
    {
        StartCoroutine(OnHit(damage));
    }

    IEnumerator OnHit(float damage)
    {
        health -= damage;
        checkHealth();
        gameMgr.DisplayDamage(health);

        yield return new WaitForSeconds(2);

        gameMgr.HideDamage();
    }

    // Powerup Helper Functions

    public void healPlayer(float amount)
    {
        health += amount;
        checkHealth();
        gameMgr.BuffHealth(health);
    }

    public void buffWeapons(float amount)
    {
        playerWeapons.buffWeapons(amount);
    }

    public void buffMaxHealth(float amount)
    {
        maxHealth += amount;
        gameMgr.BuffMaxHealth(maxHealth);
        Debug.Log("Buff Max Health: " + amount);
    }

    public void buffMoveSpeed(float amount)
    {
        playerMov.walkSpeed = playerMov.walkSpeed + amount;
        Debug.Log("Buff Move Speed: " + amount);
    }

    public void buffDashCooldown(float amount)
    {
        playerMov.dashCooldown = playerMov.dashCooldown - amount;
        Debug.Log("Buff Dash Cooldown: -" + amount);
    }

    public void buffJumpForce(float amount)
    {
        playerMov.jumpForce = playerMov.jumpForce + amount;
        Debug.Log("Buff Jump Force: " + amount);
    }
}