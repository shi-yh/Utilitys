using UnityEngine;

[System.Serializable]
public class MapConfig
{
    public Vector2Int mapSize;

    [Range(0, 1)] public float obsRate;

    public Vector2 mapMaxSize;

    public GenerateType generateType;
}