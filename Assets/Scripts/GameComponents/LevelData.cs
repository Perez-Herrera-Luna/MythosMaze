using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public int level;
    public int maxEnemies;
    public float maxEnemyPower;
    public int maxPowerups;
}
