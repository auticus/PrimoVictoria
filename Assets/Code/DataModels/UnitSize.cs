using System;

namespace PrimoVictoria.DataModels
{
    /// <summary>
    /// Class that represents a container that holds all of the available Unit Sizes that a unit can exist within
    /// </summary>
    [Serializable]
    public class UnitSize
    {
        public UnitSizeType UnitType;
        public int StandCount; //how many stands make up the unit.  Stands will hold models on them.  Units are composed of multiple stands
    }
}