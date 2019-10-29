using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LlamaZooTestMapTool.Controllers;
using LlamaZooTestMapTool.Model.Extensions;

namespace LlamaZooTestMapTool.Helpers
{
    /// <summary>
    /// Gizmo Helper class
    /// Only exists to get a quick peek at the map the user is working on the map tool
    /// Needs to be a Mono class since OnDrawGizmos can only be called from a mono class in the scene.
    /// 
    /// TODO improve not to have access to the whole map service. It makes it more coupled.
    /// </summary>
    public class GizmoHelper : MonoBehaviour
    {
        public IMapService MapService;

        public Color EmptyColor = Color.white;
        public Color WallColor = Color.black;
        public bool RoomGizmo = false;

        private void Awake()
        {
            Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            if (MapService == null) return;
            if (MapService.Map == null || MapService.Map.Count == 0) return;

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
                    Gizmos.color = (currentCell.Value == 1) ? WallColor : EmptyColor;
                    Vector3 pos = new Vector3(-MapService.Width / 2 + currentCell.PosX + .5f, 0, -MapService.Height / 2 + currentCell.PosY + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                });
            }
        }
    }
}