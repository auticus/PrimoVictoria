using PrimoVictoria.Assets.Code.Models.Parameters;
using PrimoVictoria.DataModels;
using PrimoVictoria.Models;
using UnityEngine;

namespace PrimoVictoria.Assets.Code.Models.Utilities
{
    public static class UnitFactory
    {
        public static Unit BuildUnit(GameObject owningCollection, string name, int unitID, UnitData data, Vector3 location, Vector3 rotation, int stands, int standCountWidth)
        {
            var unit = new GameObject(name);
            var unitComponent = unit.AddComponent<Unit>();
            unit.transform.parent = owningCollection.transform;
            unitComponent.Data = data;  //obviously we need to not hardcode this, its for setup testing only - requires that this element exist on the editor window
            unitComponent.ID = unitID;
            
            var initializationParameters = new UnitInitializationParameters(owner: unit, unitID, name, data, 
                standCount: stands, 
                horizontalStandCount: standCountWidth, 
                unitLocation: location, 
                rotation: rotation,
                standVisible: false
                );

            unitComponent.InitializeUnit(initializationParameters);
            return unitComponent;
        }

        public static Stand BuildStand(StandInitializationParameters parms)
        {
            var stand = new GameObject($"Stand_{parms.Data.Name}_{parms.Index}");
            var standModel = stand.AddComponent<Stand>();

            stand.transform.SetParent(parms.Parent.transform);

            standModel.InitializeStand(parms);
            return standModel;
        }
    }
}
