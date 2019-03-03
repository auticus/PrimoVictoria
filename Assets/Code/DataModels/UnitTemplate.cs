using UnityEngine;
using System.Collections.Generic;

namespace PrimoVictoria.DataModels
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "New Unit", order = 1)]
    public class UnitTemplate : ScriptableObject
    {
        public string Name = "New Unit Name";
        public FactionTemplate Faction;
        public UnitTypes UnitType = UnitTypes.Unknown;
        public List<UnitSize> AvailableUnitSizes = new List<UnitSize>();
        public UnitStatistics Statistics = new UnitStatistics();
        public List<SpecialRules> UnitSpecialRules = new List<SpecialRules>();
        public List<EquipmentTemplate> UnitEquipment = new List<EquipmentTemplate>();
    }
}