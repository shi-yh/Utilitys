

using System.Collections.Generic;

public class Room
{
      public List<Coord> tiles;

      public List<Coord> edgeTiles;

      public List<Room> connectionRooms;


      public int roomSize;

      public Room(List<Coord> roomTiles, bool[,] mapBlock)
      {
            tiles = roomTiles;
            
            
            
            
      }
      


}