using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using LlamaZooTestMapTool.Model;
using LlamaZooTestMapTool.Model.Extensions;

namespace LlamaZooTestMapTool.Controllers
{
    /// <summary>
    /// This is the main class that will handle most of the map functionalities
    /// Which can be listed as:
    ///     Generating Map
    ///     Smoothing Map (Using Cellular Automata)
    ///     Identifying Rooms
    ///     Remove rooms in which are too small (Called Islands)
    ///     Ensure room connectivity
    ///     Generate Mesh from logical Map
    ///     Create a prefab from Mesh
    /// </summary>
    public class MapService : IMapService
    {
        //The map service has a member of the MapMeshCreator class inside so it can forward the calls to it like a decorator pattern
        IMapMeshCreator MeshCreator;

        public MapService()
        {
            MeshCreator = new MapMeshCreator();
        }

        public MapService(List<List<int>> mockMap)
        {
            MeshCreator = new MapMeshCreator();
            Map = mockMap;
            Width = Map.Count;
            Height = Map[0].Count;
            Variation = 1;
        }

        public List<Room> Rooms { get; private set; }

        public List<List<int>> Map { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Variation { get; private set; }

        /// <summary>
        /// Generates de logic map
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="variation"></param>
        public void GenerateMap(int width, int height, int variation)
        {
            Width = width;
            Height = height;
            Variation = variation;

            Rooms = new List<Room>();

            Map = new List<List<int>>();
            for (int i = 0; i < width; i++)
            {
                Map.Add(new List<int>());
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || i == width - 1 || j == 0 || j == height - 1) //Border?
                    {
                        Map[i].Add(1);
                    }
                    else
                    {
                        Map[i].Add(UnityEngine.Random.Range(0, variation + 1));//Maybe do something to make it start more filled?     
                    }
                }
            }
        }

        /// <summary>
        /// Will apply an iteration of the Cellular Automata to make some sense of the map
        /// Can be applied multiple times for better effect
        /// </summary>
        public void ApplyAutomata()
        {
            List<List<int>> CachedMap = new List<List<int>>(Map);

            Map.IterateThrough((currentCell) =>
            {
                Dictionary<int, int> NeighbourTypeCount = new Dictionary<int, int>();

                Map.IterateThroughNeighbours(currentCell.PosX, currentCell.PosY, (neighbourCell) =>
                {
                    if (NeighbourTypeCount.ContainsKey(Map[neighbourCell.PosX][neighbourCell.PosY]))
                    {
                        NeighbourTypeCount[Map[neighbourCell.PosX][neighbourCell.PosY]]++;
                    }
                    else
                    {
                        NeighbourTypeCount.Add(Map[neighbourCell.PosX][neighbourCell.PosY], 1);
                    }
                });

                CachedMap[currentCell.PosX][currentCell.PosY] = AutomataRuling(CachedMap[currentCell.PosX][currentCell.PosY], NeighbourTypeCount);
            });

            Map = CachedMap;
        }

        /// <summary>
        /// Applies the ruling to propagate neighbouring
        /// </summary>
        /// <param name="neighbouringTypesCount"></param>
        /// <returns></returns>
        public int AutomataRuling(int defaultValue, Dictionary<int, int> neighbouringTypesCount)
        {
            if (neighbouringTypesCount.Count == 0)
                return defaultValue;

            int ReturnValue = -1;

            if (neighbouringTypesCount.ContainsKey(0) && neighbouringTypesCount[0] > 4)
                ReturnValue = 0;
            else if (neighbouringTypesCount.ContainsKey(1) && neighbouringTypesCount[1] > 4)
                ReturnValue = 1;

            if (ReturnValue == -1)
                ReturnValue = defaultValue;

            return ReturnValue;
        }

        /// <summary>
        /// Find Rooms will use a flood-like algorithm to identify rooms, it can find rooms of any type
        /// </summary>
        /// <param name="roomtype"></param>
        public void FindRooms(int roomtype)
        {
            List<Cell> VisitedCells = new List<Cell>();
            Rooms = new List<Room>();

            Map.IterateThrough((currentCell) =>
            {
                if (currentCell.Value != roomtype || VisitedCells.Any(x => x.PosX == currentCell.PosX && x.PosY == currentCell.PosY)) return;
                VisitedCells.Add(currentCell);

            //Flood
            var RoomCells = Map.FloodFrom(currentCell, (visitedCell) =>
                {
                //Add Flooded Cells to visited
                VisitedCells.Add(visitedCell);
                });

            //Make a Room with floodedCells
            Rooms.Add(new Room(Rooms.Count, RoomCells));
            });
        }

        /// <summary>
        /// Will find the rooms and remove those in which are too small to be relevant.
        /// I called such rooms "Islands"
        /// </summary>
        public void ClearIslands()
        {
            for (int i = 0; i <= Variation; i++)
            {
                FindRooms(i);

                for (int j = 0; j < Rooms.Count; j++)
                {
                    if (Rooms[j].RoomCells.Count < (Width <= Height ? Width : Height))
                    {
                        Rooms[j].IterateThroughCells((roomCell) =>
                        {
                            Map[roomCell.PosX][roomCell.PosY] = roomCell.Value == 1 ? 0 : 1;
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Will ensure that all rooms must be connected, as such generating single room.
        /// </summary>
        /// <param name="roomType"></param>
        public void ConnectRooms(int roomType)
        {
            while (Rooms.Count > 1)
            {
                Room RoomA = Rooms[0];
                Room RoomB = Rooms[1];

                Cell RoomACellClosestToRoomB = RoomA.RoomCells[UnityEngine.Random.Range(0, RoomA.RoomCells.Count)]; //Need to pick one just to start
                Cell RoomBCellClosestToRoomA = RoomB.RoomCells.OrderBy(x => x.DistanceBetween(RoomACellClosestToRoomB)).FirstOrDefault();
                RoomACellClosestToRoomB = RoomA.RoomCells.OrderBy(x => x.DistanceBetween(RoomBCellClosestToRoomA)).FirstOrDefault();

                Cell CursorCell = RoomACellClosestToRoomB;

                while (CursorCell.DistanceBetween(RoomBCellClosestToRoomA) > 0)
                {
                    var CursorNeighbours = CursorCell.GetNeighbouringCells(Map);
                    CursorNeighbours.ForEach(x => Map[x.PosX][x.PosY] = roomType);
                    CursorCell = CursorNeighbours.OrderBy(x => x.DistanceBetween(RoomBCellClosestToRoomA)).FirstOrDefault();
                }

                FindRooms(roomType);
            }
        }

        /// <summary>
        /// Easy logs, formatted to see the map in the log
        /// </summary>
        /// <returns></returns>
        public string LogMap()
        {
            string LogString = "";

            Map.IterateThrough((currentCell) =>
            {
                LogString += Map[currentCell.PosX][currentCell.PosY].ToString() + " ";
                if (currentCell.PosY == Map[currentCell.PosX].Count - 1) LogString += System.Environment.NewLine;
            });

            return LogString;
        }

        /// <summary>
        /// Forwards to inner Mesh creator
        /// TODO improve this by having a base implementation that doesn't need map for Map service so it sends its own.
        /// </summary>
        /// <param name="map"></param>
        public void GenerateMeshFromMap(List<List<int>> map)
        {
            MeshCreator.GenerateMeshFromMap(map);
        }

        /// <summary>
        /// Forwards to inner Mesh creator
        /// </summary>
        /// <param name="map"></param>
        public void SetPrefabValue(int key, GameObject prefab)
        {
            MeshCreator.SetPrefabValue(key, prefab);
        }
    }
}