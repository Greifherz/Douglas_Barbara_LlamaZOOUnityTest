using UnityEngine;
using UnityEditor;
using LlamaZooTestMapTool.Helpers;
using LlamaZooTestMapTool.Controllers;

namespace LlamaZooTestMapTool.Editor
{

    /// <summary>
    /// Map tool class
    /// This is the editor window class to makes the tool a tool
    /// This is mostly a view class and should not possess any intelligence beyond display intelligence
    /// </summary>
    public class MapTool : EditorWindow
    {
        [Range(0, 256)] //Limited, don't want my computer to fry
        int MapWidth = 10;

        [Range(0, 256)] //Limited, don't want my computer to fry
        int MapHeight = 10;

        GizmoHelper GizmoHelper = null;
        IMapService MapService = new MapService();

        GameObject WallPrefab;

        bool Preview = false;

        /// <summary>
        /// Init function of the EditorWindow class.
        /// Need to initiate everything here
        /// </summary>
        [MenuItem("Window/Map Tool")]
        static void Init()
        {
            MapTool MapToolWindow = (MapTool)EditorWindow.GetWindow(typeof(MapTool));

            MapToolWindow.Show();

        }

        /// <summary>
        /// OnGUI from the inspector of the window
        /// Here is where the view code comes
        /// </summary>
        void OnGUI()
        {
            GUILayout.Label("Map Generator tool", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            MapWidth = NonNegativeIntField("Map Width", MapWidth);
            MapHeight = NonNegativeIntField("Map Height", MapHeight);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            if (MapService.Map != null && MapService.Map.Count > 0)
            {
                Preview = EditorGUILayout.BeginToggleGroup("Preview", Preview);
                if (Preview)
                {
                    CreateGizmoHelper();
                    GizmoHelper.WallColor = EditorGUILayout.ColorField("Wall Color", GizmoHelper.WallColor);
                    GizmoHelper.EmptyColor = EditorGUILayout.ColorField("Empty Color", GizmoHelper.EmptyColor);
                    if (MapService.Rooms.Count > 0)
                        GizmoHelper.RoomGizmo = EditorGUILayout.Toggle("See rooms", GizmoHelper.RoomGizmo);
                }
                else
                {
                    DestroyGizmoHelper();
                }
                EditorGUILayout.EndToggleGroup();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Map Generation Actions");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate"))
            {
                MapService.GenerateMap(MapWidth, MapHeight, 1);
                DestroyGizmoHelper();
            }
            if (GUILayout.Button("Clear"))
            {
                MapService = new MapService();

                DestroyGizmoHelper();
            }
            EditorGUILayout.EndHorizontal();

            if (MapService.Map != null && MapService.Map.Count > 0)
            {
                EditorGUILayout.LabelField("Map Processing Actions");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Smooth"))
                {
                    MapService.ApplyAutomata();
                    DestroyGizmoHelper();
                }
                if (GUILayout.Button("Find Rooms"))
                {
                    MapService.FindRooms(0);
                    DestroyGizmoHelper();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                if (MapService.Rooms.Count > 0)
                {
                    EditorGUILayout.LabelField("Room Processing Actions");
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Remove Islands"))
                    {
                        MapService.ClearIslands();
                        DestroyGizmoHelper();
                    }
                    if (GUILayout.Button("Ensure Connectivity"))
                    {
                        MapService.ConnectRooms(0);
                        DestroyGizmoHelper();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                EditorGUILayout.LabelField("Mesh Actions");

                WallPrefab = (GameObject)EditorGUILayout.ObjectField("Wall Prefab", WallPrefab, typeof(GameObject), true);
                if (WallPrefab != null)
                {
                    if (GUILayout.Button("Generate Mesh"))
                    {
                        Preview = false;
                        DestroyGizmoHelper();

                        MapService.SetPrefabValue(1, WallPrefab);
                        MapService.SetPrefabValue(0, null);
                        MapService.GenerateMeshFromMap(MapService.Map);
                    }
                }
            }
        }

        /// <summary>
        /// Create a Gizmo to show what has been done
        /// </summary>
        private void CreateGizmoHelper()
        {
            if (GizmoHelper != null) return;

            GizmoHelper = new GameObject("GizmoHelper").AddComponent<GizmoHelper>();
            GizmoHelper.MapService = MapService;
        }

        /// <summary>
        /// Dispatch the Gizmo so it can update
        /// </summary>
        private void DestroyGizmoHelper()
        {
            if (GizmoHelper == null) return;

            DestroyImmediate(GizmoHelper.gameObject);
            GizmoHelper = null;
        }

        /// <summary>
        /// OnDestroy it destroys the gizmo not to cause any leaks.
        /// </summary>
        private void OnDestroy()
        {
            DestroyGizmoHelper();
        }

        /// <summary>
        /// Trick for not allowing user to input negative values
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        int NonNegativeIntField(string name, int fieldValue)
        {
            int FieldInt = EditorGUILayout.IntField(name, fieldValue);
            if (fieldValue < 0)
                return 0;
            return FieldInt;
        }
    }
}