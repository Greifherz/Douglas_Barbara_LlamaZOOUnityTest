using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace LlamaZooTestMapTool.Model.Extensions
{
    public static class MapExtensions
    {
        public static void IterateThrough(this List<List<int>> self, Action<Cell> iteration)
        {
            for (int i = 1; i < self.Count - 1; i++)//Starts at 1 and ends at count-1 to skip border
            {
                for (int j = 1; j < self[i].Count - 1; j++)
                {
                    iteration(new Cell(self[i][j], i, j));
                }
            }
        }

        public static void IterateThroughEverything(this List<List<int>> self, Action<Cell> iteration)
        {
            for (int i = 0; i < self.Count; i++)
            {
                for (int j = 0; j < self[i].Count; j++)
                {
                    iteration(new Cell(self[i][j], i, j));
                }
            }
        }

        public static void IterateThroughNeighbours(this List<List<int>> self, int x, int y, Action<Cell> iteration)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (i < 0 || i >= self.Count) continue;
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if ((i == x && j == y) || j < 0 || j >= self[i].Count) continue;
                    iteration(new Cell(self[i][j], i, j));
                }
            }
        }

        public static List<Cell> GetNeighbours(this List<List<int>> self, Cell main)
        {
            List<Cell> Neighbours = new List<Cell>();
            self.IterateThroughNeighbours(main.PosX, main.PosY, (Neighbour) =>
              {
                  Neighbours.Add(Neighbour);
              });

            return Neighbours;
        }

        public static List<Cell> FloodFrom(this List<List<int>> self, Cell startingCell, Action<Cell> OnEachCell = null)
        {
            List<Cell> RoomCells = new List<Cell>();
            Queue<Cell> ToVisitCells = new Queue<Cell>();

            ToVisitCells.Enqueue(startingCell);

            do
            {
                Cell CurrentCell = ToVisitCells.Dequeue();
                RoomCells.Add(CurrentCell);

                foreach (var neighbouringCell in CurrentCell.GetNeighbouringCells(self))
                {
                    if (neighbouringCell.Value != startingCell.Value) continue;
                    if (RoomCells.Any(cell => cell.PosX == neighbouringCell.PosX && cell.PosY == neighbouringCell.PosY)) continue; //Overriding Equals was too slow
                    if (ToVisitCells.Any(cell => cell.PosX == neighbouringCell.PosX && cell.PosY == neighbouringCell.PosY)) continue; //Overriding Equals was too slow
                    ToVisitCells.Enqueue(neighbouringCell);

                    OnEachCell?.Invoke(neighbouringCell);
                }

            } while (ToVisitCells.Count > 0);

            //string Logstring = "";
            //foreach(var cell in RoomCells)
            //{
            //    Logstring += cell.ToString() + ",";
            //}

            //Debug.Log("Roomcells-> " +Logstring);

            return RoomCells;
        }
    }
}
