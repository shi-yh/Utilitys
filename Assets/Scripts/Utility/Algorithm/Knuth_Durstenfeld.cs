using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Coord
{
    public int x;

    public int y;

    public Coord(int x, int y)
    {
        this.x = x;

        this.y = y;
    }
}

public partial class Utility
{
    public static T[] ShuffleCoord<T>(T[] dataArray)
    {
        for (int i = 0; i < dataArray.Length; i++)
        {
            int random = Random.Range(i, dataArray.Length);

            (dataArray[random], dataArray[i]) = (dataArray[i], dataArray[random]);
        }

        return dataArray;
    }
}