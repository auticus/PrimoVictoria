using UnityEngine;
using System;

namespace PrimoVictoria.DataModels
{
    /// <summary>
    /// Scriptable Object that represents a container that holds all of the available Unit Sizes that a unit can exist within
    /// </summary>
    [CreateAssetMenu(fileName = "New Unit Size", menuName = "New Unit Size", order = 4)]
    public class UnitSize : ScriptableObject
    {
        public UnitTypes UnitType;
        public int StandCount; //how many stands make up the unit.  Stands will hold models on them.  Units are composed of multiple stands
    }
}