using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlamaZooTestMapTool.Model
{
    /// <summary>
    /// Stuct to contain the room concept
    /// It has the cells that makes it a room and an index so it can be compared
    /// Used as a struct instead of class for performance
    /// </summary>
    public struct Room
    {
        public Room(List<Cell> roomCells)
        {
            RoomCells = roomCells;
            RoomIndex = 0;
        }

        public Room(int roomIndex, List<Cell> roomCells)
        {
            RoomIndex = roomIndex;
            RoomCells = roomCells;
        }

        public int RoomIndex { get; private set; }
        public Color RoomColor => GetColorByIndex(RoomIndex);
        public List<Cell> RoomCells { get; private set; }

        private Color GetColorByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return Color.cyan;
                case 1:
                    return Color.green;
                case 2:
                    return Color.red;
                case 3:
                    return Color.blue;
                case 4:
                    return Color.yellow;
                default:
                    return new Color(UnityEngine.Random.Range(0, 256), UnityEngine.Random.Range(0, 256), UnityEngine.Random.Range(0, 256));
            }
        }
    }
}
