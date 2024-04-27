using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using UnityEditor;
using UnityEngine;

public class Player_Damage : MonoBehaviour
{
    private PlayerManager playerMgr;

    private bool invulnerable = false;
    private bool playerHit = false;
    private bool playerInDanger = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMgr = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
    }

    void OnTriggerStay(Collider other)
    {
       // Debug.Log(other.gameObject.name);
        if(other.gameObject.CompareTag("skeleton"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            //Debug.Log("collider triggered");
            if(!invulnerable && enemy.executedAttack && enemy.isAttacking)
            {      
                invulnerable = true;
                Debug.Log("Skeleton attacked player");
                playerMgr.playerHit(enemy.attackDamage);
                StartCoroutine(invulnerableDelay(2f));
            }
        }
    }

    IEnumerator invulnerableDelay(float dur)
    {
        yield return new WaitForSeconds(dur);
        invulnerable = false;
    }
}
