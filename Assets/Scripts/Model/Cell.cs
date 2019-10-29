using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlamaZooTestMapTool.Model
{
    /// <summary>
    /// Cell struct to hold the concept of Cell
    /// It has its position and its value on the map
    /// Is a struct for perfomance and simplicity, immutability is desired
    /// </summary>
    public struct Cell
    {
        public Cell(int value, int posX, int posY)
        {
            Value = value;
            PosX = posX;
            PosY = posY;
        }

        public int Value { get; private set; }
        public int PosX { get; private set; }
        public int PosY { get; private set; }

        public override string ToString()
        {
            return "[" + PosX + "," + PosY + "]";
        }

        //Was Too Slow
        //public override bool Equals(object obj)
        //{
        //    Cell Other = (Cell)obj;
        //    return PosX == Other.PosX && PosY == Other.PosY;
        //}
    }
}
