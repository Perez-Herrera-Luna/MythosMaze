using UnityEngine;

[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    //Name
    public new string name;

    //Health
    public float health;
    public float maxHealth;

    // Procedural Generation
    public int minNumPerArena;
    public int maxNumPerArena;
    public bool canDropPowerup;

    //Attacking
    public float attackDamage;
    public float maxAttackDamage;
    public float attackSpeed;
    public bool hasAttacked = false;

    //states
    public float sightRange, attackRange;
    public bool playerInSight, playerAttackable;

    //movement
    public float wanderSpeed; 
    public float chaseSpeed; 
    public float turnSpeed; 
    public float groundDrag; 
    public float airMultiplier; 
    public float roamRange; 
    public float flyingHeightMax; 
    public bool canFly;
}
