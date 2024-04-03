using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public int level;

    // level procedural generation data
    public int gridRows;
    public int gridCols;

    public int pathWidth;  

    public int maxNumArenas;
    public int arenaDistanceBuff;
    public int drunkenRatio;
    public int maxPathLength;
}
