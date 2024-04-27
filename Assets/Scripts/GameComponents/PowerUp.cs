using System;
//using System.Collections;
//using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public float amount;
    public string powerupName;

    private PowerupManager powerupMgr;

    // Start is called before the first frame update
    void Start()
    {
        powerupMgr = GameObject.Find("PowerupManager").GetComponent<PowerupManager>();
    }

    void OnTriggerEnter(Collider hit)
    {
        if(hit.gameObject.CompareTag("player"))
        {
            Debug.Log("Player picked up powerup: " + powerupName);

            powerupMgr.ActivatePowerup(powerupName, amount);

            Destroy(gameObject);
        }
    }
}
