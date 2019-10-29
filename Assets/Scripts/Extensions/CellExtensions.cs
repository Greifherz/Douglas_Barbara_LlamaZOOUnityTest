using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlamaZooTestMapTool.Model.Extensions
{
    public static class CellExtensions
    {
        public static List<Cell> GetNeighbouringCells(this Cell self, List<List<int>> map)
        {
            var NeighbouringCells = new List<Cell>();

            map.IterateThroughNeighbours(self.PosX, self.PosY, (neighbour) =>
            {
                NeighbouringCells.Add(neighbour);
            });

            return NeighbouringCells;
        }

        public static float DistanceBetween(this Cell self, Cell other)
        {
            //return Vector2.Distance(new Vector2(self.PosX, self.PosY), new Vector2(other.PosX, other.PosY)); //UnityWay

            return Mathf.Sqrt(Mathf.Pow((float)other.PosX - (float)self.PosX, 2) + Mathf.Pow((float)other.PosY - (float)self.PosY, 2)); //Math way, faster
        }
    }
}
