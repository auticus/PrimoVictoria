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
        public int ModelsPerStand = 1;
        public UnitStatistics Statistics = new UnitStatistics();
        public List<SpecialRules> UnitSpecialRules = new List<SpecialRules>();
        public List<Equipment> UnitEquipment = new List<Equipment>();

        /// <summary>
        /// The visual model
        /// </summary>
        public GameObject UnitMesh;

        /// <summary>
        /// The size of the selection cursor under the mesh when the unit is selected
        /// </summary>
        public double SelectionOrthoSize = 0.7;

        public float WalkSpeed = 2.0f;  //the navagent speed when walking (2.0 is an infantry man walk)
        public float RunSpeed = 5.0f;  //the navagent speed when running (5.0 is an infantry man run)
    }
}