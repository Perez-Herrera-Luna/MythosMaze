using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public string name;
    public bool isThrown;
    public float damage;
    public float speed;
    public float cooldown;
}
