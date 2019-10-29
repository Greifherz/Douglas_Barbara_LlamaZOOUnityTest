using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LlamaZooTestMapTool.Controllers;
using LlamaZooTestMapTool.Model.Extensions;
//using NUnit.Framework;

namespace LlamaZooTestMapTool.Tests
{
    /// <summary>
    /// Test monobehaviour class
    /// Used for a quick writing and quick seeing testing
    /// </summary>
    public class MapTests : MonoBehaviour
    {
        // I wanted to use NUnit and have the Unit's test framework on it, but since the assignment specifies for no 3rd parties I'll make do with this monobehaviour

        //Wont comment much on this since it is pretty straightforward and later on I moved most of it to MapTool. 
        //However I'll leave it here in case any performance testing is needed with the profiler.

        IMapService MapService = new MapService();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A)) //Testing Map generator basic function
            {
                MapService.GenerateMap(3, 3, 1);

                Debug.Log(MapService.LogMap()); //This string should represent a 3x3 matrix only with 0, and 1s

                MapService.GenerateMap(5, 5, 2);

                Debug.Log(MapService.LogMap()); //This string should represent a 5x5 matrix with 0s,1s and 2s

                MapService.GenerateMap(10, 10, 3);

                Debug.Log(MapService.LogMap()); //This string should represent a 10x10 matrix with 0s,1s,2s and 3s
            }

            if (Input.GetKeyDown(KeyCode.S)) //Testing Smoothing with really large map
            {
                Debug.Log("Testing Smoothing with really large map!");

                MapService.GenerateMap(256, 128, 1);

                StartCoroutine(AutomataRoutine());

            }

            if (Input.GetKeyDown(KeyCode.D)) //Generate Mock Map
            {
                Debug.Log("Generate Mock Map!");

                var mock = new List<List<int>> {
                new List<int> { 1, 1, 1, 1, 1 },
                new List<int> { 1, 0, 0, 0, 1 },
                new List<int> { 1, 0, 1, 0, 1 },
                new List<int> { 1, 0, 0, 0, 1 },
                new List<int> { 1, 1, 1, 1, 1 },
                new List<int> { 1, 0, 0, 0, 1 },
                new List<int> { 1, 0, 1, 0, 1 },
                new List<int> { 1, 0, 0, 0, 1 },
                new List<int> { 1, 1, 1, 1, 1 },
                new List<int> { 1, 0, 0, 0, 1 },
                new List<int> { 1, 0, 1, 0, 1 },
                new List<int> { 1, 0, 0, 0, 1 },
                new List<int> { 1, 1, 1, 1, 1 }
            };

                MapService = new MapService(mock);

                Debug.Log("Done!");
            }

            if (Input.GetKeyDown(KeyCode.F)) //Generate Usable Map
            {
                Debug.Log("Generate Usable Map!");
                MapService.GenerateMap(50, 50, 1);

                for (int i = 0; i < 3; i++)
                    MapService.ApplyAutomata();


                Debug.Log("Done!");
            }

            if (Input.GetKeyDown(KeyCode.G)) //Testing Regions
            {
                Debug.Log("Testing Regions!");

                MapService.FindRooms(0);

                Debug.Log("Done!");
            }

            if (Input.GetKeyDown(KeyCode.Q)) //Testing removing Islands
            {
                Debug.Log("Testing removing Islands!");
                MapService.ClearIslands();

                MapService.FindRooms(0);

                Debug.Log("Done!");
            }

            if (Input.GetKeyDown(KeyCode.W)) //Testing Connecting Rooms
            {
                Debug.Log("Testing Connecting Rooms!");
                MapService.FindRooms(0);

                MapService.ConnectRooms(0);

                Debug.Log("Done!");
            }

            if (Input.GetKeyDown(KeyCode.Space)) //Toggle RoomGizmo
            {
                Debug.Log("Done!");
                RoomGizmo = !RoomGizmo;
            }
        }

        IEnumerator AutomataRoutine() //2 frames wait time to buffer and see things changing
        {
            yield return null;

            for (int i = 0; i < 10; i++)
            {
                yield return null;
                yield return null;

                MapService.ApplyAutomata();
            }

            Debug.Log("Done!");
        }

        public Color WalkableColor;
        public Color WallColor;

        bool RoomGizmo = false;

        void OnDrawGizmos()
        {
            if (MapService.Map != null)
            {
                if (RoomGizmo)
                {
                    MapService.Rooms.ForEach(room => room.RoomCells.ForEach((currentCell) =>
                    {
                        Gizmos.color = room.RoomColor;
                        Vector3 pos = new Vector3(-MapService.Width / 2 + currentCell.PosX + .5f, 0, -MapService.Height / 2 + currentCell.PosY + .5f);
                        Gizmos.DrawCube(pos, Vector3.one);
                    }));
                }
                else
                {
                    MapService.Map.IterateThroughEverything((currentCell) =>
                    {
                        Gizmos.color = (currentCell.Value == 1) ? WallColor : WalkableColor;
                        Vector3 pos = new Vector3(-MapService.Width / 2 + currentCell.PosX + .5f, 0, -MapService.Height / 2 + currentCell.PosY + .5f);
                        Gizmos.DrawCube(pos, Vector3.one);
                    });
                }
            }
        }
    }
}