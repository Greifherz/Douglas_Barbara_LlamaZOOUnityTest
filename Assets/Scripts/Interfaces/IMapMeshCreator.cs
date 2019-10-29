using System.Collections.Generic;
using UnityEngine;

namespace LlamaZooTestMapTool.Controllers
{
    public interface IMapMeshCreator
    {
        void GenerateMeshFromMap(List<List<int>> map);
        void SetPrefabValue(int key, GameObject prefab);
    }
}