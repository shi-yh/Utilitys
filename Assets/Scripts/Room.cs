using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Room : IComparable<Room>
{
    public List<Coord> tiles;

    public List<Coord> edgeTiles;

    public List<Room> connectionRooms;

    public int roomSize;

    public bool isAccessibleFromMainRoom;

    public bool isMainRoom;


    public Room()
    {
    }

    public Room(List<Coord> roomTiles, bool[,] mapBlock)
    {
        tiles = roomTiles;
        roomSize = tiles.Count;
        connectionRooms = new List<Room>();

        edgeTiles = new List<Coord>();

        for (int k = 0; k < tiles.Count; k++)
        {
            //如果周围存在墙，那么它是边界
            for (int i = tiles[k].x - 1; i <= tiles[k].x + 1; i++)
            {
                for (int j = tiles[k].y - 1; j <= tiles[k].y + 1; j++)
                {
                    if (mapBlock[i, j])
                    {
                        edgeTiles.Add(tiles[k]);
                    }
                }
            }
        }
    }


    public static void ConnectRoom(Room a, Room b)
    {
        if (a.isAccessibleFromMainRoom)
        {
            b.SetAccessibleFromMainRoom();
        }
        else if (b.isAccessibleFromMainRoom)
        {
            a.SetAccessibleFromMainRoom();
        }


        a.connectionRooms.Add(b);
        b.connectionRooms.Add(a);
    }

    public bool IsConnect(Room otherRoom)
    {
        return connectionRooms.Contains(otherRoom);
    }

    public int CompareTo(Room otherRoom)
    {
        return otherRoom.roomSize.CompareTo(roomSize);
    }

    public void SetAccessibleFromMainRoom()
    {
        if (!isAccessibleFromMainRoom)
        {
            isAccessibleFromMainRoom = true;

            for (int i = 0; i < connectionRooms.Count; i++)
            {
                connectionRooms[i].SetAccessibleFromMainRoom();
            }
        }
    }
}