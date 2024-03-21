using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeScript : MonoBehaviour
{
    public float life = 4;
    void Awake()
    {
        Destroy(gameObject, life);
    }

    void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
