using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager inst;

    private GameManager gameMgr;
    private PowerupManager powerupMgr;

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

    public void buffMaxHealth(float amount)
    {

    }

    public void buffMoveSpeed(float amount)
    {
        // buff player movement walk speed (everything derives from that)
    }

    public void buffWeapons(float amount)
    {

    }

}