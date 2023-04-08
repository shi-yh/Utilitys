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

    public int obsCount;

    public float obsMinHeight, obsMaxHeight;

    public Color frontGroundColor, backGroundCOlor;


    [Range(0, 1)] public float outLinePersent;

    private List<Coord> _coords = new List<Coord>();

    private Queue<Coord> _shuffleCoords = new Queue<Coord>();


    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        CreateTile();

        _shuffleCoords = new Queue<Coord>(Utility.ShuffleCoord(_coords.ToArray()));

        CreateObs();
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
    }

    private void CreateObs()
    {
        for (int i = 0; i < obsCount; i++)
        {
            float obsHeight = Mathf.Lerp(obsMinHeight, obsMaxHeight, Random.Range(0f, 1f));


            Coord coord = GetRandomPos();
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