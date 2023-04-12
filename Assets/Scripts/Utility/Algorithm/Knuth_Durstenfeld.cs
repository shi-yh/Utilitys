using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public struct Coord
{
    public int x;

    public int y;

    public Coord(int x, int y)
    {
        this.x = x;

        this.y = y;
    }


    public static bool operator !=(Coord coord0, Coord coord1)
    {
        return !(coord0 == coord1);
    }

    public static bool operator ==(Coord coord0, Coord coord1)
    {
        return coord0.x == coord1.x && coord0.y == coord1.y;
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

    public static T[] ShuffleCoord<T>(T[,] dataArray)
    {
        int xLength = dataArray.GetLength(0);

        int yLength = dataArray.GetLength(1);

        T[] array = new T[xLength * yLength];


        for (int i = 0; i < xLength; i++)
        {
            for (int j = 0; j < yLength; j++)
            {
                int randomX = Random.Range(i, xLength);
                int randomY = Random.Range(j, yLength);

                (dataArray[randomX, randomY], dataArray[i, j]) = (dataArray[i, j], dataArray[randomX, randomY]);

                array[i * yLength + j] = dataArray[i, j];
            }
        }


        return array;
    }

    public static List<Coord> GetLineByBresenham(Coord begin, Coord end)
    {
        List<Coord> result = new List<Coord>();

        int x = begin.x;

        int y = begin.y;

        int dx = end.x - begin.x;

        int dy = end.y - begin.y;

        //从x开始增加还是从y开始增加
        bool inverted = false;

        //x差值的正负，当x增长的时候，是向上还是向下
        int step = Math.Sign(dx);
        //Y差值的正负，当y增长的时候，是向上还是向下
        int gradientStep = Math.Sign(dy);
        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);
            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        
        for (int i = 0; i < longest; i++)
        {
            result.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;

            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }

                gradientAccumulation -= longest;
            }
        }


        return result;
    }
}