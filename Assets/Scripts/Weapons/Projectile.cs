using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody rb;
    float currDamage;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // damage and speed from calling ShootingWeapon attributes
    public void Shoot(float damage, float speed)
    {
        rb.velocity = transform.forward * speed;
        currDamage = damage;
    }

    void OnTriggerEnter(Collider hit)
    {
        // check obj tag for: player, enemy, wall
        // bool collider.gameObject.CompareTag(str)
    }
}
