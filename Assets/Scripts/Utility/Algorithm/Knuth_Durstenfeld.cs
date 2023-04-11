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
}