using UnityEngine;

[CreateAssetMenu]
public class MonsterData : ScriptableObject
{
    public string name;
    public float attackDamage;
    public float attackSpeed;
    public float health;

    public bool canFly;
    // attackType
}
