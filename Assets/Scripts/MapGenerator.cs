using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GenerateType
{
    FloodFill,

    Cell,
}

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapConfig _config;

    public GameObject tile;

    public Transform mapParent;

    public GameObject obsObj;

    public float obsMinHeight, obsMaxHeight;

    public Color frontGroundColor, backGroundCOlor;

    public GameObject navMeshObstacle;

    [Range(0, 1)] public float outLinePersent;


    /// <summary>
    /// 实际地图
    /// </summary>
    private Coord[,] _coords;

    private Queue<Coord> _shuffleCoords = new Queue<Coord>();

    private bool[,] _mapObstacles;

    private Coord _mapCenter;
    [SerializeField] private int _smoothCount;


    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        _coords = new Coord[_config.mapSize.x, _config.mapSize.y];

        CreateTile();
        CreateNavObs();


        switch (_config.generateType)
        {
            case GenerateType.FloodFill:
            {
                _shuffleCoords = new Queue<Coord>(Utility.ShuffleCoord(_coords));
                CreateFloodObs();
                break;
            }
            case GenerateType.Cell:
            {
                CreateCellObs();

                break;
            }
        }
    }

    private Dictionary<Coord, GameObject> _obs = new();

    public float yieTime = 0.5f;

    private void CreateCellObs()
    {
        _mapObstacles = new bool[_config.mapSize.x, _config.mapSize.y];


        Random.InitState(Random.Range(0, 100));
        //第一步生成无规则
        for (int i = 0; i < _config.mapSize.x; i++)
        {
            for (int j = 0; j < _config.mapSize.y; j++)
            {
                if (i == 0 || i == _config.mapSize.x - 1 || j == 0 || j == _config.mapSize.y - 1)
                {
                    SetObstacle(i, j, true);
                }
                else
                {
                    SetObstacle(i, j, Random.value < _config.obsRate);
                }
            }
        }

        //细胞机处理
        StartCoroutine(SmoothMap());
    }

    private List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();

        bool[,] mapFlags = new bool[_config.mapSize.x, _config.mapSize.y];

        //要搜索的这一片区域的匹配类型
        bool tileType = _mapObstacles[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();

        queue.Enqueue(_coords[startX, startY]);

        mapFlags[startX, startY] = true;

        while (queue.Count > 0)
        {
            Coord curTile = queue.Dequeue();

            tiles.Add(curTile);

            for (int i = curTile.x - 1; i <= curTile.x + 1; i++)
            {
                for (int j = curTile.y - 1; j <= curTile.y + 1; j++)
                {
                    if (IsInMapRange(i, j) && (j == curTile.y || i == curTile.x))
                    {
                        if (!mapFlags[i, j] && _mapObstacles[i, j] == tileType)
                        {
                            mapFlags[i, j] = true;

                            queue.Enqueue(_coords[i, j]);
                        }
                    }
                }
            }
        }

        return tiles;
    }


    private List<List<Coord>> GetRegions(bool block)
    {
        List<List<Coord>> regions = new List<List<Coord>>();


        bool[,] mapFlag = new Boolean[_config.mapSize.x, _config.mapSize.y];

        for (int i = 0; i < _config.mapSize.x; i++)
        {
            for (int j = 0; j < _config.mapSize.y; j++)
            {
                //首先这块地面没有被搜索过，其次它的类型和需要的类型一致
                if (!mapFlag[i, j] && _mapObstacles[i, j] == block)
                {
                    List<Coord> newRegion = GetRegionTiles(i, j);

                    regions.Add(newRegion);

                    for (int k = 0; k < newRegion.Count; k++)
                    {
                        mapFlag[newRegion[k].x, newRegion[k].y] = true;
                    }
                }
            }
        }

        return regions;
    }

    private void RejectMap()
    {
        List<List<Coord>> wallRegions = GetRegions(true);

        int blockSize = 10;

        for (int i = 0; i < wallRegions.Count; i++)
        {
            if (wallRegions[i].Count < blockSize)
            {
                for (int j = 0; j < wallRegions[i].Count; j++)
                {
                    SetObstacle(wallRegions[i][j].x, wallRegions[i][j].y, false);
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(false);

        int roomSize = 20;

        for (int i = 0; i < roomRegions.Count; i++)
        {
            if (roomRegions[i].Count < roomSize)
            {
                for (int j = 0; j < roomRegions[i].Count; j++)
                {
                    SetObstacle(roomRegions[i][j].x, roomRegions[i][j].y, true);
                }
            }
        }
    }


    private IEnumerator SmoothMap()
    {
        yield return new WaitForSeconds(3);

        for (int i = 0; i < _smoothCount; i++)
        {
            for (int j = 0; j < _config.mapSize.x; j++)
            {
                for (int k = 0; k < _config.mapSize.y; k++)
                {
                    Coord coord = _coords[j, k];

                    int neighborWalls = GetSurroundWallCount(coord);

                    if (neighborWalls > 4)
                    {
                        //如果周围墙的数量大于4个，则将自己封闭
                        SetObstacle(j, k, true);
                    }
                    else if (neighborWalls < 4)
                    {
                        SetObstacle(j, k, false);
                    }
                }
            }

            if (yieTime > 0)
            {
                yield return new WaitForSeconds(yieTime);
            }
        }

        yield return new WaitForSeconds(3);

        RejectMap();
    }

    /// <summary>
    /// 获取某个瓦片周围的墙的数量
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    private int GetSurroundWallCount(Coord coord)
    {
        int wallCount = 0;

        for (int neighbourX = coord.x - 1; neighbourX <= coord.x + 1; neighbourX++)
        {
            for (int neighbourY = coord.y - 1; neighbourY <= coord.y + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    //排除自身
                    if (neighbourX != coord.x ||
                        neighbourY != coord.y)
                    {
                        wallCount += _mapObstacles[neighbourX, neighbourY] ? 1 : 0;
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }


    private void CreateNavObs()
    {
        GameObject navMeshForward = Instantiate(navMeshObstacle,
            new Vector3(0, 1f, (_config.mapMaxSize.y - _config.mapSize.y) / 4),
            Quaternion.identity, mapParent);

        navMeshForward.transform.localScale =
            new Vector3(_config.mapSize.x, 0, (_config.mapMaxSize.y - _config.mapSize.y) / 2);

        GameObject navMeshBackward = Instantiate(navMeshObstacle,
            new Vector3(0, 1f, -(_config.mapMaxSize.y - _config.mapSize.y) / 4),
            Quaternion.identity, mapParent);

        navMeshBackward.transform.localScale =
            new Vector3(_config.mapSize.x, 0, -(_config.mapMaxSize.y - _config.mapSize.y) / 2);

        GameObject navMeshLeft = Instantiate(navMeshObstacle,
            -new Vector3((_config.mapMaxSize.x - _config.mapSize.x) / 4, 0, 0),
            Quaternion.identity, mapParent);

        navMeshLeft.transform.localScale =
            new Vector3((_config.mapMaxSize.x - _config.mapSize.x) / 2, 1f, _config.mapSize.y);

        GameObject navMeshRight = Instantiate(navMeshObstacle,
            new Vector3((_config.mapMaxSize.x - _config.mapSize.x) / 4, 0, 0),
            Quaternion.identity, mapParent);

        navMeshRight.transform.localScale =
            new Vector3((_config.mapMaxSize.x - _config.mapSize.x) / 2, 1f, _config.mapSize.y);
    }

    private void CreateTile()
    {
        for (int i = 0; i < _config.mapSize.x; i++)
        {
            for (int j = 0; j < _config.mapSize.y; j++)
            {
                Coord coord = new Coord(i, j);

                Vector3 pos = GetPosByCoord(coord);
                GameObject go = Instantiate(tile, pos, Quaternion.Euler(90, 0, 0), mapParent);
                go.transform.localScale *= (1 - outLinePersent);
                _coords[i, j] = coord;
            }
        }


        _mapCenter = new Coord(_config.mapSize.x / 2, _config.mapSize.y / 2);
    }

    private void CreateFloodObs()
    {
        int obsCount = (int)(_config.obsRate * _config.mapSize.x * _config.mapSize.y);

        _mapObstacles = new bool[_config.mapSize.x, _config.mapSize.y];

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
                CreateObstacle(coord);
            }
        }
    }

    private void SetObstacle(int x, int y, bool isBlock)
    {
        _mapObstacles[x, y] = isBlock;

        if (isBlock)
        {
            if (!_obs.ContainsKey(_coords[x, y]) || _obs[_coords[x, y]] == null)
            {
                CreateObstacle(_coords[x, y]);
            }
        }
        else
        {
            if (_obs.ContainsKey(_coords[x, y]) && _obs[_coords[x, y]] != null)
            {
                Destroy(_obs[_coords[x, y]]);
                _obs[_coords[x, y]] = null;
            }
        }
    }


    private void CreateObstacle(Coord coord)
    {
        float obsHeight = Mathf.Lerp(obsMinHeight, obsMaxHeight, Random.Range(0f, 1f));
        Vector3 pos = GetPosByCoord(coord);
        pos.y = obsHeight / 2;
        GameObject go = Instantiate(obsObj, pos, Quaternion.identity, mapParent);
        go.transform.localScale = new Vector3((1 - outLinePersent), obsHeight, 1 - outLinePersent);


        // MeshRenderer mesh = go.GetComponent<MeshRenderer>();
        //
        // Material material = mesh.material;
        //
        // float colorPersent = coord.y / _config.mapSize.y;
        //
        // material.color = Color.Lerp(frontGroundColor, backGroundCOlor, colorPersent);
        //
        // mesh.material = material;

        _obs[coord] = go;
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
                    if ((x == 0 || y == 0) && IsInMapRange(neighborX, neighborY))
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


        return _config.mapSize.x * _config.mapSize.y - curObsCount == accessibleCount;
    }

    private Vector3 GetPosByCoord(Coord coord)
    {
        return new Vector3(-_config.mapSize.x / 2 + 0.5f + coord.x, 0, -_config.mapSize.y / 2 + 0.5f + coord.y);
    }

    private bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < _config.mapSize.x && y >= 0 && y < _config.mapSize.y;
    }


    private Coord GetRandomPos()
    {
        Coord result = _shuffleCoords.Dequeue();

        _shuffleCoords.Enqueue(result);

        return result;
    }
}