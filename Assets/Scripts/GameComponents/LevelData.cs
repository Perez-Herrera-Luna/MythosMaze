using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public int level;

    // level procedural generation data
    public int gridRows;
    public int gridCols;
    
    // hold radius of inner, middle, and outer rings used in determining arena door count
    public float[] gridRings = new float[3];    

    public int maxNumArenas;
    public int maxTriesGenArena;
    public int maxTriesGenLoc;
    public int maxPathLength;
}
