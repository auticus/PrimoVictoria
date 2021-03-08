using UnityEngine;

namespace PrimoVictoria.Assets.Code.Models.Parameters
{
    public struct UnitInitializationParameters
    {
        public GameObject ContainingGameObject { get; }
        public int UnitID { get; }
        public int StandCount { get; }
        public Vector3 UnitLocation { get;  }
        public Vector3 Rotation { get; }
        public bool StandVisible { get; }
        public bool ModelMeshesVisible { get; }

        public UnitInitializationParameters(GameObject containingGameObject, int unitID, int standCount, Vector3 unitLocation,
            Vector3 rotation, bool standVisible, bool modelMeshesVisible)
        {
            ContainingGameObject = containingGameObject;
            UnitID = unitID;
            StandCount = standCount;
            UnitLocation = unitLocation;
            Rotation = rotation;
            StandVisible = standVisible;
            ModelMeshesVisible = modelMeshesVisible;
        }
    }
}
