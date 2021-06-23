using PrimoVictoria.DataModels;
using PrimoVictoria.Models;
using UnityEngine;

namespace PrimoVictoria.Assets.Code.Models.Parameters
{
    public class StandInitializationParameters
    {
        public int Index { get; }
        public Unit Parent { get; }
        public UnitData Data { get; }
        public Vector3 Location { get; }
        public Vector3 Rotation { get; }
        public int FileIndex { get; }
        public int RankIndex { get; }
        public bool StandVisible { get; }
        public float Spacing { get; }

        public StandInitializationParameters(int index, Unit parent, UnitData data, Vector3 location, Vector3 rotation, 
                                            int fileIndex, int rankIndex, bool standVisible, float spacing)
        {
            Index = index;
            Parent = parent;
            Data = data;
            Location = location;
            Rotation = rotation;
            FileIndex = fileIndex;
            RankIndex = rankIndex;
            StandVisible = standVisible;
            Spacing = spacing;
        }
    }
}
