using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeapon : MonoBehaviour
{
    public WeaponData weapon;

    Rigidbody rb;

    // current stats calculated using weapon data + current player buffs
    float currDamage;
    float currSpeed;
    float currCooldown;

    // called by playerManager when weapon is set as active
    public void updateWeaponDamage(float weaponDamageBuff) => currDamage = weapon.damage + weaponDamageBuff;
    public void updateWeaponSpeed(float weaponSpeedBuff) => currSpeed = weapon.speed + weaponSpeedBuff;
    public void updateWeaponCooldown(float cooldownReduction) => currCooldown = weapon.cooldown - cooldownReduction;

    // Start is called before the first frame update
    void Start()
    {
        currDamage = weapon.damage;
        currSpeed = weapon.speed;
        currCooldown = weapon.cooldown;

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        // ‘shoot’ weapon in forward direction
        rb.velocity = transform.forward * currSpeed;

        // need to make new weapon appear in player's hand
    }

    public void OnTriggerEnter()
    {
        // check obj tag for: player, enemy, wall
        // bool collider.gameObject.CompareTag(str)
    }
}
