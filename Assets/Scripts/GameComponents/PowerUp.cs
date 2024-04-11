using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public float buffAmount;
    public float buffDurration;
    object[] buffObjectArray = new object[1];
    public string buffMethodName;

    PlayerManager playerMgr;
    Type playerMgrClassType;
    MethodInfo methodReference;

    public PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        float[] buff = {buffAmount};
        Array.Copy(buff, buffObjectArray, buff.Length);
        playerData = GameObject.Find("Player").GetComponent<PlayerData>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider hit)
    {
        if(hit.gameObject.CompareTag("player"))
        {
            // Invokes method with name "buffMethodName" within PlayerManager class using System.Reflection
            playerMgr = hit.gameObject.GetComponent<PlayerManager>();
            playerMgrClassType = playerMgr.GetType();
            methodReference = playerMgrClassType.GetMethod(buffMethodName);
            methodReference.Invoke(playerMgr, buffObjectArray);

            Debug.Log(buffMethodName + "(" + buffAmount + ")");

            playerData.powerUpName = buffMethodName;
            playerData.powerUpDuration = buffDurration;
            playerData.powerUpAmount = buffAmount;

            Destroy(gameObject);
        }
    }
}
