using UnityEngine;

namespace PrimoVictoria.Assets.Code.Models.Parameters
{
    public struct UnitInitializationParameters
    {
        public GameObject ContainingGameObject { get; }
        public int UnitID { get; }

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

        public UnitInitializationParameters(GameObject containingGameObject, int unitID, int standCount, int horizontalStandCount, Vector3 unitLocation,
            Vector3 rotation, bool standVisible, bool modelMeshesVisible)
        {
            ContainingGameObject = containingGameObject;
            UnitID = unitID;
            StandCount = standCount;
            HorizontalStandCount = horizontalStandCount;
            UnitLocation = unitLocation;
            Rotation = rotation;
            StandVisible = standVisible;
            ModelMeshesVisible = modelMeshesVisible;
        }
    }
}
