using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LlamaZooTestMapTool.Model.Extensions;

namespace LlamaZooTestMapTool.Controllers
{
    /// <summary>
    /// Class that handles the creating of meshes
    /// The interface is needed because there is improvement to be done here
    /// Instead of instantiating objects to form a mesh from them, since it would lead to redundant polygons it would be Ideal to just calculate the mesh
    /// Since it takes a lot of time and this "works" and it is well decoupled I'll leave it at that
    /// </summary>
    public class MapMeshCreator : IMapMeshCreator
    {
        //Stores prefabs for each cell in a dictionary for quick to read operations
        private Dictionary<int, GameObject> PrefabPerValue = new Dictionary<int, GameObject>();

        public GameObject MeshHolder;

        [SerializeField] private Vector2[] MeshUV;
        [SerializeField] private Vector3[] MeshVertices;
        [SerializeField] private int[] MeshTriangles;

        public MapMeshCreator()
        {

        }

        /// <summary>
        /// Sets the prefab for the map-desired value
        /// The default usage of the tool is walls to 1 and nothing to 0
        /// but it can be easily changed
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prefab"></param>
        public void SetPrefabValue(int key, GameObject prefab)
        {
            if (PrefabPerValue.ContainsKey(key))
            {
                PrefabPerValue[key] = prefab;
            }
            else
                PrefabPerValue.Add(key, prefab);
        }

        /// <summary>
        /// Given a logical map it creates a single combined mesh from the previously set prefabs-to-value relation in the dictionary
        /// After done, it saves both a prefab with the single mesh and its mesh on a Resources folder for later usage
        /// 
        /// TODO improve not overwriting the prefabs and meshes by indexing them or letting the user name them
        /// </summary>
        /// <param name="map"></param>
        public void GenerateMeshFromMap(List<List<int>> map)
        {
            if (PrefabPerValue.Count == 0) throw new System.Exception("Cannot generate without no prefabs specified");
            if (MeshHolder != null) GameObject.DestroyImmediate(MeshHolder);
            MeshHolder = new GameObject("Mesh Holder");
            MeshHolder.transform.position = new Vector3();

            float StartingX = -map.Count * 0.5f;
            float StartingY = -map[0].Count * 0.5f;

            List<Mesh> MeshListToCombine = new List<Mesh>();

            var CellCombines = new List<CombineInstance>();

            Material Material = null;

            map.IterateThroughEverything((cell) =>
            {
                var Prefab = PrefabPerValue[cell.Value];
                if (Prefab == null) return;
                var PhysicalCell = GameObject.Instantiate(Prefab, new Vector3(StartingX + cell.PosX, 0, StartingY + cell.PosY), Quaternion.identity, MeshHolder.transform);
                var CellCombine = new CombineInstance();
                CellCombine.mesh = PhysicalCell.GetComponent<MeshFilter>().sharedMesh;
                CellCombine.transform = PhysicalCell.transform.localToWorldMatrix;
                CellCombines.Add(CellCombine);
                if (Material == null)
                    Material = PhysicalCell.GetComponent<MeshRenderer>().sharedMaterial;
            });

            var Mesh = new Mesh();
            Mesh.CombineMeshes(CellCombines.ToArray());
            Mesh = SerializeMesh(Mesh);


            GameObject.DestroyImmediate(MeshHolder);
            MeshHolder = new GameObject("CombinedMeshHolder");
            MeshHolder.AddComponent<MeshRenderer>().material = Material;
            MeshHolder.AddComponent<MeshFilter>().mesh = Mesh;

            AssetDatabase.CreateAsset(Mesh, "Assets/Resources/Output/Meshes/CombinedMapMesh.asset");
            AssetDatabase.SaveAssets();

            PrefabUtility.SaveAsPrefabAssetAndConnect(MeshHolder, "Assets/Resources/Output/Maps/GeneratedMap.prefab", InteractionMode.UserAction);

        }

        /// <summary>
        /// Helper function to have the combined mesh become serializable and thus able to become an asset
        /// </summary>
        /// <param name="meshToSerialize"></param>
        /// <returns></returns>
        private Mesh SerializeMesh(Mesh meshToSerialize)
        {
            MeshUV = meshToSerialize.uv;
            MeshVertices = meshToSerialize.vertices;
            MeshTriangles = meshToSerialize.triangles;

            Mesh mesh = new Mesh();
            mesh.vertices = MeshVertices;
            mesh.triangles = MeshTriangles;
            mesh.uv = MeshUV;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

    }
}