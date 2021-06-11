using PrimoVictoria.DataModels;
using UnityEngine;

namespace PrimoVictoria.Assets.Code.Models.Parameters
{
    public struct UnitInitializationParameters
    {
        public int UnitID { get; }
        public string Name { get; }
        public GameObject ContainingGameObject { get; private set; }

        /// <summary>
        /// How many total stands are in the unit
        /// </summary>
        public int StandCount { get; }

        /// <summary>
        /// How many stands are in a full rank
        /// </summary>
        public int HorizontalStandCount { get; }

        /// <summary>
        /// The location of the first stand in the unit
        /// </summary>
        public Vector3 UnitLocation { get;  }
        public Vector3 Rotation { get; }
        public bool StandVisible { get; }
        public bool ModelMeshesVisible { get; }
        public UnitData Data { get; }

        public UnitInitializationParameters(GameObject owner, int unitID, string name, UnitData data, int standCount, int horizontalStandCount, Vector3 unitLocation,
            Vector3 rotation, bool standVisible, bool modelMeshesVisible)
        {
            ContainingGameObject = owner;
            StandCount = standCount;
            HorizontalStandCount = horizontalStandCount;
            UnitLocation = unitLocation;
            Rotation = rotation;
            StandVisible = standVisible;
            ModelMeshesVisible = modelMeshesVisible;
            Data = data;
            UnitID = unitID;
            Name = name;
        }
    }
}
