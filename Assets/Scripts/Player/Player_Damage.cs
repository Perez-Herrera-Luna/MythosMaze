using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using UnityEditor;
using UnityEngine;

public class Player_Damage : MonoBehaviour
{
    public static float playerHealth = 10.0f;
    private float maxHealth = 10.0f;

    private GameManager gameMgr;
    private Monster monster_script;

    // Start is called before the first frame update
    void Start()
    {
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        checkHealth();
    }

    private void checkHealth()
    {
        if(playerHealth > maxHealth)
        {
            playerHealth = maxHealth;
        }
        if(playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("Player Died");
            gameOver();
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("melee_enemy"))
        {
            Debug.Log("collider triggered");
            monster_script = other.GetComponent<Monster>();
            if(monster_script.hasAttacked)
            {
                Debug.Log("Enemy Attacked");
                StartCoroutine(OnHit(2));
            }
            
        }
    }

    IEnumerator OnHit(int damage)
    {
        playerHealth -= damage;
        Debug.Log("Health: " + playerHealth);
        gameMgr.DisplayDamage();

        yield return new WaitForSeconds(2);

        gameMgr.HideDamage();
    }

    private void gameOver()
    {
        //death code//
        gameMgr.GameOver();
    }


}
