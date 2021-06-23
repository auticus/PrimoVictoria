using PrimoVictoria.DataModels;
using UnityEngine;

namespace PrimoVictoria.Models.Parameters
{
    public struct UnitInitializationParameters
    {
        public int UnitID { get; }
        public string Name { get; }

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

        public UnitData Data { get; }

        public UnitInitializationParameters(int unitID, string name, UnitData data, 
            int standCount, int horizontalStandCount, Vector3 unitLocation, Vector3 rotation, bool standVisible)
        {
            StandCount = standCount;
            HorizontalStandCount = horizontalStandCount;
            UnitLocation = unitLocation;
            Rotation = rotation;
            Data = data;
            UnitID = unitID;
            StandVisible = standVisible;
            Name = name;
        }
    }
}
