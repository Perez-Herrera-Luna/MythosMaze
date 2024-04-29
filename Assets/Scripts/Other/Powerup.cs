using System;
//using System.Collections;
//using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public PowerupData powerupData;

    private PowerupManager powerupMgr;

    // Start is called before the first frame update
    void Start()
    {
        powerupMgr = GameObject.Find("PowerupManager").GetComponent<PowerupManager>();
    }

    void OnTriggerExit(Collider hit)
    {
        if(hit.gameObject.CompareTag("player"))
        {
            powerupMgr.ActivatePowerup(powerupData.powerupName, powerupData.valuePerLevel);

            Destroy(gameObject);
        }
    }
}
