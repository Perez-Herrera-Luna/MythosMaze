using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LevelDataTests
{
    private LevelData[] levelGrids;

    [SetUp]
    public void Setup()
    {
        levelGrids = Resources.LoadAll<LevelData>("DataAssets");
    }

    [UnityTest]
    public IEnumerator LevelData_CheckForUniqueLevelNumbers()
    {
        bool allUniqueLevelNumbers = true;
        HashSet<int> levelNumbers = new HashSet<int>();

        foreach(LevelData grid in levelGrids)
        {
            if(!levelNumbers.Add(grid.level)){
                Debug.Log("Error: duplicate level number: " + grid.level + " in LevelData assets");
                allUniqueLevelNumbers = false;
            }
        }

        Assert.IsTrue(allUniqueLevelNumbers);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelData_CheckForOddGridDimensions()
    {
        bool allOddDimensions = true;

        foreach(LevelData grid in levelGrids)
        {
            if(grid.gridRows % 2 == 0){
                Debug.Log("Error: level " + grid.level + " grid row value (" + grid.gridRows + ") is not odd.");
                allOddDimensions = false;
            }

            if(grid.gridCols % 2 == 0){
                Debug.Log("Error: level " + grid.level + " grid column value (" + grid.gridCols + ") is not odd.");
                allOddDimensions = false;
            }
        }

        Assert.IsTrue(allOddDimensions);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelData_CheckForPositivePathWidth()
    {
        bool allPositivePathWidths = true;

        foreach(LevelData grid in levelGrids)
        {
            if(grid.pathWidth <= 0){
                Debug.Log("Error: level + " + grid.level + " invalid pathWidth value (must be > 0).");
                allPositivePathWidths = false;
            }
        }

        Assert.IsTrue(allPositivePathWidths);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelData_CheckForCorrespondingPathPrefabWidth()
    {
        bool allPathWidthsValuesValid = true;
        HashSet<int> pathWidthValues = new HashSet<int>();
        GameObject[] paths = Resources.LoadAll<GameObject>("Prefabs/LevelGeneration/Paths");

        foreach(GameObject path in paths)
        {
            GameObject pathObject = GameObject.Instantiate(path);
            GameObject ground = GameObject.Find("Ground");

            if(ground != null)
                pathWidthValues.Add((int)ground.transform.localScale.z);

            Object.Destroy(pathObject.gameObject);
        }

        foreach(LevelData grid in levelGrids)
        {
            if(!pathWidthValues.Contains(grid.pathWidth)){
                Debug.Log("Error: no path prefab has corresponding pathWidth: " + grid.pathWidth);
                allPathWidthsValuesValid = false;
            }
        }

        Assert.IsTrue(allPathWidthsValuesValid);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelData_CheckForPositiveDrunkenRatio()
    {
        bool allPositiveRatios = true;

        foreach(LevelData grid in levelGrids)
        {
            if(grid.drunkenRatio <= 0){
                Debug.Log("Error: level + " + grid.level + " invalid drunkenRatio value (must be > 0).");
                allPositiveRatios = false;
            }
        }

        Assert.IsTrue(allPositiveRatios);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelData_CheckForPositiveMaxPathLength()
    {
        bool allPositiveMaxPathLengths = true;

        foreach(LevelData grid in levelGrids)
        {
            if(grid.maxPathLength <= 0){
                Debug.Log("Error: level + " + grid.level + " invalid maxPathLength value (must be > 0).");
                allPositiveMaxPathLengths = false;
            }
        }

        Assert.IsTrue(allPositiveMaxPathLengths);

        yield return null;
    }
}
