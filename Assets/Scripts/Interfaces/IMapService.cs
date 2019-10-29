using System.Collections.Generic;
using LlamaZooTestMapTool.Model;

namespace LlamaZooTestMapTool.Controllers
{
    public interface IMapService : IMapMeshCreator
    {
        List<Room> Rooms { get; }
        List<List<int>> Map { get; }
        int Variation { get; }
        int Width { get; }
        int Height { get; }

        void ConnectRooms(int roomType);
        void ClearIslands();
        void FindRooms(int roomType);
        void ApplyAutomata();
        int AutomataRuling(int defaultValue, Dictionary<int, int> neighbouringTypesCount);
        void GenerateMap(int width, int heigth, int variation);

        string LogMap();
    }
}