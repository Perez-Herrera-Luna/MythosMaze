using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeScript : MonoBehaviour
{
    public float life = 4.0f;
    void Awake()
    {
        Destroy(transform, life);
    }

    void OnCollisionEnter(Collision other)
    {
        Destroy(transform);
    }
}
