using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using UnityEditor;
using UnityEngine;

public class Player_Damage : MonoBehaviour
{
    public PlayerData playerData;

    private GameManager gameMgr;
    private bool invulnerable = false;
    private bool playerHit = false;
    private bool playerInDanger = false;

    // [Header("Health UI")]

    public PlayerHealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
        healthBar.SetMaxHealth(playerData.playerMaxHealth);
        healthBar.SetHealth(playerData.playerHealth);
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.SetHealth(playerData.playerHealth);
        checkHealth();
    }

    private void checkHealth()
    {
        if(playerData.playerHealth > playerData.playerMaxHealth)
        {
            playerData.playerHealth = playerData.playerMaxHealth;
        }
        if(playerData.playerHealth <= 0)
        {
            playerData.playerHealth = 0;    
            gameOver();
        }
    }

    void OnTriggerStay(Collider other)
    {
       // Debug.Log(other.gameObject.name);
        if(other.gameObject.CompareTag("skeleton"))
        {
            //Debug.Log("collider triggered");
            if(!invulnerable && other.gameObject.GetComponent<Enemy>().executedAttack && other.gameObject.GetComponent<Enemy>().isAttacking)
            {      
                invulnerable = true;
                Debug.Log("Skeleton attacked player");
                StartCoroutine(OnHit(2));
                StartCoroutine(invulnerableDelay(2f));
            }
        }
    }

    IEnumerator invulnerableDelay(float dur)
    {
        yield return new WaitForSeconds(dur);
        invulnerable = false;
    }

    IEnumerator OnHit(int damage)
    {
        playerData.playerHealth -= damage;
        Debug.Log("Player Health: " + playerData.playerHealth);
        gameMgr.DisplayDamage();

        yield return new WaitForSeconds(2);

        gameMgr.HideDamage();
    }

    private void gameOver()
    {
        //death code//
        gameMgr.GameOver();
        Debug.Log("Player Died");
    }
}
