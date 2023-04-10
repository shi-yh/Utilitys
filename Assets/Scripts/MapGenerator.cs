using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class MapGenerator : MonoBehaviour
{
    public GameObject tile;

    public Vector2 mapSize;

    public Transform mapParent;

    public GameObject obsObj;

    [Range(0, 1)] public float obsRate;

    public float obsMinHeight, obsMaxHeight;

    public Color frontGroundColor, backGroundCOlor;

    public Vector2 mapMaxSize;

    public GameObject navMeshObstacle;


    [Range(0, 1)] public float outLinePersent;

    private List<Coord> _coords = new List<Coord>();

    private Queue<Coord> _shuffleCoords = new Queue<Coord>();

    private bool[,] _mapObstacles;

    private Coord _mapCenter;


    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        CreateTile();

        _shuffleCoords = new Queue<Coord>(Utility.ShuffleCoord(_coords.ToArray()));

        CreateObs();

        CreateNavObs();
    }

    private void CreateNavObs()
    {
        GameObject navMeshForward = Instantiate(navMeshObstacle, new Vector3(0, 1f, (mapMaxSize.y - mapSize.y) / 4),
            Quaternion.identity, mapParent);

        navMeshForward.transform.localScale = new Vector3(mapSize.x, 0, (mapMaxSize.y - mapSize.y) / 2);

        GameObject navMeshBackward = Instantiate(navMeshObstacle, new Vector3(0, 1f, -(mapMaxSize.y - mapSize.y) / 4),
            Quaternion.identity, mapParent);

        navMeshBackward.transform.localScale = new Vector3(mapSize.x, 0, -(mapMaxSize.y - mapSize.y) / 2);

        GameObject navMeshLeft = Instantiate(navMeshObstacle, -new Vector3((mapMaxSize.x - mapSize.x) / 4, 0, 0),
            Quaternion.identity, mapParent);

        navMeshLeft.transform.localScale = new Vector3((mapMaxSize.x - mapSize.x) / 2, 1f, mapSize.y);

        GameObject navMeshRight = Instantiate(navMeshObstacle, new Vector3((mapMaxSize.x - mapSize.x) / 4, 0, 0),
            Quaternion.identity, mapParent);

        navMeshRight.transform.localScale = new Vector3((mapMaxSize.x - mapSize.x) / 2, 1f, mapSize.y);
    }

    private void CreateTile()
    {
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                Coord coord = new Coord(i, j);

                Vector3 pos = GetPosByCoord(coord);
                GameObject go = Instantiate(tile, pos, Quaternion.Euler(90, 0, 0), mapParent);
                go.transform.localScale *= (1 - outLinePersent);
                _coords.Add(coord);
            }
        }


        _mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);
    }

    private void CreateObs()
    {
        int obsCount = (int)(obsRate * mapSize.x * mapSize.y);

        _mapObstacles = new bool[(int)(mapSize.x), (int)(mapSize.y)];

        int curObsCount = 0;


        for (int i = 0; i < obsCount; i++)
        {
            Coord coord = GetRandomPos();

            _mapObstacles[coord.x, coord.y] = true;

            curObsCount++;

            if (coord == _mapCenter || !MapIsFullyAccessible(_mapObstacles, curObsCount))
            {
                _mapObstacles[coord.x, coord.y] = false;

                curObsCount--;
            }
            else
            {
                float obsHeight = Mathf.Lerp(obsMinHeight, obsMaxHeight, Random.Range(0f, 1f));
                Vector3 pos = GetPosByCoord(coord);
                pos.y = obsHeight / 2;
                GameObject go = Instantiate(obsObj, pos, Quaternion.identity, mapParent);
                go.transform.localScale = new Vector3((1 - outLinePersent), obsHeight, 1 - outLinePersent);


                MeshRenderer mesh = go.GetComponent<MeshRenderer>();

                Material material = mesh.material;

                float colorPersent = coord.y / mapSize.y;

                material.color = Color.Lerp(frontGroundColor, backGroundCOlor, colorPersent);

                mesh.material = material;
            }
        }
    }

    private bool MapIsFullyAccessible(bool[,] mapObstacles, int curObsCount)
    {
        //检查标志
        bool[,] mapFlags = new bool[mapObstacles.GetLength(0), mapObstacles.GetLength(1)];

        Queue<Coord> queue = new Queue<Coord>();

        mapFlags[_mapCenter.x, _mapCenter.y] = true;

        int accessibleCount = 1;

        queue.Enqueue(_mapCenter);

        while (queue.Count > 0)
        {
            Coord curTile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighborX = curTile.x + x;
                    int neighborY = curTile.y + y;

                    //排除对角线 且 不能超出地图边界
                    if ((x == 0 || y == 0) && neighborX >= 0 && neighborX < _mapObstacles.GetLength(0)
                        && neighborY >= 0 && neighborY < _mapObstacles.GetLength(1))
                    {
                        if (!mapFlags[neighborX, neighborY] && !mapObstacles[neighborX, neighborY])
                        {
                            mapFlags[neighborX, neighborY] = true;
                            accessibleCount++;
                            queue.Enqueue(new Coord(neighborX, neighborY));
                        }
                    }
                }
            }
        }


        return (int)(mapSize.x * mapSize.y - curObsCount) == accessibleCount;
    }

    private Vector3 GetPosByCoord(Coord coord)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + coord.x, 0, -mapSize.y / 2 + 0.5f + coord.y);
    }


    private Coord GetRandomPos()
    {
        Coord result = _shuffleCoords.Dequeue();

        _shuffleCoords.Enqueue(result);

        return result;
    }
}