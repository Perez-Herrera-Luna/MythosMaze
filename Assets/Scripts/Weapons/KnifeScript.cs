using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeScript : MonoBehaviour
{
    public float life = 4.0f;
    void Awake()
    {
        Object.Destroy(this.gameObject, life);
    }

    void OnCollisionEnter(Collision other)
    {
        Object.Destroy(this.gameObject);
    }
}
