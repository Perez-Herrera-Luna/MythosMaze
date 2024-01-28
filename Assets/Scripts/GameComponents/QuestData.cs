using UnityEngine;

[CreateAssetMenu]
public class QuestData : ScriptableObject
{
    public string questName;
    public string characterName;
    public string itemNeeded;
    public WeaponData rewardWeapon;
}
