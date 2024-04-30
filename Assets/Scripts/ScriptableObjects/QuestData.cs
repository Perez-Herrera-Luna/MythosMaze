using UnityEngine;

[CreateAssetMenu]
public class QuestData : ScriptableObject
{
    public string questName;
    public string characterName;
    public string itemNeeded;
    public int rewardWeapon;

    public string greeting;
    public string charIntroduction;
    public string questIntroduction;
    public string activeQuestNoItem;
    public string itemRetrieved;
    public string weaponIntroduction;
    public string questComplete;
}
