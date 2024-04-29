using UnityEngine;

[CreateAssetMenu]
public class PowerupData : ScriptableObject
{
    public string powerupName;
    public float valuePerLevel;

    public string generationLocation;
    public float generationProbability;

    public int initialLevel;
    public int minNum;
    public int maxNum;
}

