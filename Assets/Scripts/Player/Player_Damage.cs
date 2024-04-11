using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using UnityEditor;
using UnityEngine;

public class Player_Damage : MonoBehaviour
{
    public PlayerData playerData;

    private GameManager gameMgr;
    
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

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if(other.gameObject.CompareTag("skeleton"))
        {
            Debug.Log("collider triggered");
            if(other.gameObject.GetComponent<Enemy>().isAttacking)
            {
                Debug.Log("Enemy Attacked");
                StartCoroutine(OnHit(2));
            }
        }
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
