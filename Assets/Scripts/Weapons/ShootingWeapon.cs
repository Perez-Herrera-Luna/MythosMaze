using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingWeapon : MonoBehaviour
{
    public WeaponData weapon;

    // current stats calculated using weapon data + current player buffs
    float currDamage;
    float currSpeed;
    float currCooldown;

    // called by playerManager when weapon is set as active
    public void updateWeaponDamage(float weaponDamageBuff) => currDamage = weapon.damage + weaponDamageBuff;
    public void updateWeaponSpeed(float weaponSpeedBuff) => currSpeed = weapon.speed + weaponSpeedBuff;
    public void updateWeaponCooldown(float cooldownReduction) => currCooldown = weapon.cooldown - cooldownReduction;

    public GameObject projectilePrefab;
    GameObject projectileInstance;

    // Start is called before the first frame update
    void Start()
    {
        currDamage = weapon.damage;
        currSpeed = weapon.speed;
        currCooldown = weapon.cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        // instantiates projectile at weapon location
        projectileInstance = Instantiate(projectilePrefab, transform.position, transform.rotation);
        // ‘shoot’ projectile in forward direction
        // projectileInstance.GetComponent<Projectile>.Shoot(currDamage, currSpeed);
    }
}
