using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager inst;
    private void Awake()
    {
        inst = this;
    }

    //Player's stats will be stored here
    [Header("Movement")]
    public float moveSpeed; // Player's movement speed

    public float groundDrag; // Player's drag when on the ground

    public float jumpForce; // Player's jump force
    public float jumpCooldown; // Player's jump cooldown
    public float airMultiplier; // Player's movement speed multiplier when in the air
    bool canJump;

    [Header("Health")]
    public float playerHealth;

    [Header("Attack")]
    public float attackSpeed;
    public float attackDamage;

    // methods to be called by powerup type
    public void buffAttackSpeed(float buff) => attackSpeed += buff;
    public void buffAttackDamage(float buff) => attackDamage += buff;


    [Header("Weapons")]
    public int numWeapons;  // number of weapons player is current holding [can be 0, 1, or 2]
    public int activeWeapon;    // index of active weapon
    public WeaponData[] weapons = new WeaponData[2];
    public float weaponDamageBuff = 0;
    public float weaponSpeedBuff = 0;
    public float weaponCooldownReduction = 0;

    // methods to be called by powerup type
    public void buffWeaponDamage(float buff) => weaponDamageBuff += buff;
    public void buffWeaponSpeed(float buff) => weaponSpeedBuff += buff;
    public void buffWeaponCooldown(float buff) => weaponCooldownReduction += buff;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
