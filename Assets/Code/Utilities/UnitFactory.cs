using PrimoVictoria.DataModels;
using PrimoVictoria.Models;
using PrimoVictoria.Models.Parameters;
using UnityEngine;

namespace PrimoVictoria.Utilities
{
    public static class UnitFactory
    {
        public static Unit BuildUnit(GameObject unitsCollection, string name, int unitID, UnitData data, Vector3 location, Vector3 rotation, int stands, int standCountWidth)
        {
            var unitComponent = unitsCollection.AddComponent<Unit>();
            unitComponent.Data = data;  //obviously we need to not hardcode this, its for setup testing only - requires that this element exist on the editor window
            unitComponent.ID = unitID;
            
            var initializationParameters = new UnitInitializationParameters(unitID, name, data, 
                standCount: stands, 
                horizontalStandCount: standCountWidth, 
                unitLocation: location, 
                rotation: rotation,
                standVisible: true
                );

            unitComponent.InitializeUnit(initializationParameters);
            return unitComponent;
        }

        public static Stand BuildStand(StandInitializationParameters parms)
        {
            var stand = new GameObject($"Stand_{parms.Data.Name}_{parms.Index}") {layer = StaticResources.MINIATURES_LAYER};

            var standModel = stand.AddComponent<Stand>();

            stand.transform.SetParent(parms.Parent.transform);

            standModel.InitializeStand(parms);
            return standModel;
        }
    }
}
