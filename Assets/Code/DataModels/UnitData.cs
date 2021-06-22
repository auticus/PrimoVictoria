using UnityEngine;
using System.Collections.Generic;

namespace PrimoVictoria.DataModels
{
    /// <summary>
    /// Data template for units which represent the soldiers and pieces of the game
    /// </summary>
    [CreateAssetMenu(fileName = "New Unit", menuName = "New Unit", order = 1)]
    public class UnitData : ScriptableObject
    {
        public string Name = "New Unit Name";
        public Faction Faction;
        public UnitTypes UnitType = UnitTypes.Unknown;
        public UnitStatistics Statistics = new UnitStatistics();
        public List<SpecialRules> UnitSpecialRules = new List<SpecialRules>();
        public List<Equipment> UnitEquipment = new List<Equipment>();

        /// <summary>
        /// The visual model(s) representing the warriors
        /// </summary>
        public List<GameObject> UnitMeshes; //todo: because multiples can be put here need some type of data element on this object that defines what each element index is (normal, standard, etc)

        public float WalkSpeed = 2.0f;  //the navagent speed when walking (2.0 is an infantry man walk)
        public float RunSpeed = 5.0f;  //the navagent speed when running (5.0 is an infantry man run)
        public float WheelSpeed = 25.0f; //the speed multiplier when wheeling
    }
}