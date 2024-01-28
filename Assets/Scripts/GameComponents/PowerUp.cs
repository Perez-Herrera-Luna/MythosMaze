using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public float buffAmount;
    object[] buffObjectArray = new object[1];
    public string buffMethodName;

    PlayerManager playerMgr;
    Type playerMgrClassType;
    MethodInfo methodReference;

    // Start is called before the first frame update
    void Start()
    {
        float[] buff = {buffAmount};
        Array.Copy(buff, buffObjectArray, buff.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider hit)
    {
        if(hit.gameObject.CompareTag("player"))
        {
            playerMgrClassType = playerMgr.GetType();
            methodReference = playerMgrClassType.GetMethod(buffMethodName);
            methodReference.Invoke(playerMgr, buffObjectArray);
        }
    }
}
