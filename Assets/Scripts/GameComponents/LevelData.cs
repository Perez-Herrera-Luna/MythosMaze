using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public int level;

    // level procedural generation data
    public int gridRows;
    public int gridCols;
    public int gridScale;
    
    // hold radius of inner and outer rings used in determining arena door count
    public float[] gridRings;    

    public int maxNumArenas;
    public int maxTriesGenArena;
    public int arenaDistanceBuff;
    public int maxTriesGenLoc;
    public int drunkenRatio;
    public int maxPathLength;
}
