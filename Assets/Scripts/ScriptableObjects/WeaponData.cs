using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public bool isThrown;
    public float damage;
    public float speed;
    public float cooldown;
    public float lifeTime;
}
