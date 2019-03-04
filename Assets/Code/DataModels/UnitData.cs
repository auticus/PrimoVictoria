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
        public List<UnitSize> AvailableUnitSizes = new List<UnitSize>();
        public UnitStatistics Statistics = new UnitStatistics();
        public List<SpecialRules> UnitSpecialRules = new List<SpecialRules>();
        public List<Equipment> UnitEquipment = new List<Equipment>();
    }
}