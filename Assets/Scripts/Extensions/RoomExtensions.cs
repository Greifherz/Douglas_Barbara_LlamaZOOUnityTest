using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace LlamaZooTestMapTool.Model.Extensions
{
    public static class RoomExtensions
    {
        public static void IterateThroughCells(this Room self, Action<Cell> iterator)
        {
            for (int i = 0; i < self.RoomCells.Count; i++)
            {
                iterator(self.RoomCells[i]);
            }
        }
    }
}