using PrimoVictoria.DataModels;
using PrimoVictoria.Models;
using UnityEngine;

namespace PrimoVictoria.Assets.Code.Models.Parameters
{
    public class StandInitializationParameters
    {
        public Unit Parent { get; }
        public UnitData Data { get; }
        public Vector3 Location { get; }
        public Vector3 Rotation { get; }
        public int FileIndex { get; }
        public int RankIndex { get; }
        public bool StandVisible { get; }
        public bool ModelMeshesVisible { get; }

        public StandInitializationParameters(Unit parent, UnitData data, Vector3 location, Vector3 rotation, 
                                            int fileIndex, int rankIndex, bool standVisible, bool modelMeshesVisible)
        {
            Parent = parent;
            Data = data;
            Location = location;
            Rotation = rotation;
            FileIndex = fileIndex;
            RankIndex = rankIndex;
            StandVisible = standVisible;
            ModelMeshesVisible = modelMeshesVisible;
        }
    }
}
