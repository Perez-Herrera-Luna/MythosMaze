using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player_Damage : MonoBehaviour
{
    Rigidbody rb;

    public static float playerHealth = 10.0f;
    private float maxHealth = 10.0f;

    private SceneManager sceneMgr;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sceneMgr = GameObject.Find("SceneManager").GetComponent<SceneManager>();
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
            Debug.Log("Player hit!");
            //player hit
            StartCoroutine(OnHit(2));
        }
    }

    IEnumerator OnHit(int damage)
    {
        playerHealth -= damage;
        Debug.Log("Health: " + playerHealth);
        sceneMgr.DisplayDamage();

        yield return new WaitForSeconds(2);

        sceneMgr.HideDamage();
    }

    private void gameOver()
    {
        //death code//
        sceneMgr.GameOver();
    }


}
