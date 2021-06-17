using System;
using System.Collections.Generic;
using System.Linq;
using PrimoVictoria.Assets.Code.Models;
using PrimoVictoria.Assets.Code.Models.Utilities;
using PrimoVictoria.Controllers;
using PrimoVictoria.DataModels;
using PrimoVictoria.Models;
using UnityEngine;

namespace PrimoVictoria.Assets.Code.Controllers
{
    /// <summary>
    /// Controller class that is responsible for issuing orders to the units in the game
    /// </summary>
    public class UnitController : MonoBehaviour
    {
        public EventHandler<StandLocationArgs> OnSelectedUnitLocationChanged;

        [SerializeField] Projectors Projectors; //collection of all Projectors
        [SerializeField] List<UnitData> Faction_0_Units;

        private GameObject UnitsCollection { get; set; }
        private const string UNITS_GAMEOBJECT = "Units";

        private Unit _selectedUnit;
        public Unit SelectedUnit
        {
            get => _selectedUnit;
            private set
            {
                if (_selectedUnit != null && value != null && _selectedUnit.ID == value.ID)
                    return;

                if (_selectedUnit != null)
                {
                    _selectedUnit.Unselect();
                }

                _selectedUnit = value;

                if (_selectedUnit != null)
                {
                    _selectedUnit.Select(Projectors,
                        isFriendly: true); //todo: tell if friend or not and not hardcode it to always be friend

                    _selectedUnit.OnLocationChanged += (sender, args) =>
                    {
                        OnSelectedUnitLocationChanged?.Invoke(sender, args);
                    };
                }
                else
                {
                    OnSelectedUnitLocationChanged?.Invoke(this, new StandLocationArgs(Vector3.zero));
                }
            }
        }

        void Awake()
        {
            UnitsCollection = GameObject.Find(UNITS_GAMEOBJECT);
            if (UnitsCollection == null)
            {
                UnitsCollection = new GameObject(UNITS_GAMEOBJECT) {layer = GameManager.MINIATURES_LAYER};
            }
        }

        public void LoadUnits()
        {
            //todo: this whole thing is hardcoded and is only for dev purposes, this will need redefined after development to not hardcode the unit types
            //todo: a loading screen of some kind will populate what units are present, for right now this is just loaded with a test unit

            //IMPORTANT
            //the Game Manager instance in the editor will have had units added to it (which is why there is no code here adding any but we are referencing them)
            var location = new Vector3(50, 0, 50);

            //rotation is based on the Y axis
            var rotation = new Vector3(0, 45, 0);
            var unit = UnitFactory.BuildUnit(UnitsCollection, "Men at Arms", 1, Faction_0_Units[0], 
                location, rotation, stands: 5, standCountWidth: 5);
            unit.ToggleDiagnostic(true);

            /*
            //UNIT 2
            location = new Vector3(90.0f, 0.4f, 80.7f);
            rotation = new Vector3(0,0,0);
            UnitFactory.BuildUnit(unitsCollection, "Men At Arms 2", 2, Faction_0_Units[0], location, rotation, stands: 1, standCountWidth: 1);
            */
        }

        public void SetSelectedUnit(Unit unit)
        {
            SelectedUnit = unit;
        }

        public void SelectUnit(int unitID)
        {
            if (UnitsCollection == null)
            {
                Debug.LogWarning("UnitController::UnitsCollection was NULL!");
                SelectedUnit = null;
                return;
            }

            SelectedUnit = GetSelectedUnit(UnitsCollection, unitID);
        }

        public void MoveSelectedUnitToPoint(Vector3 worldPosition, bool isRunning)
        {
            if (SelectedUnit != null)
                SelectedUnit.Move(worldPosition, isRunning: isRunning);
        }

        public void UnitWheeling(IEnumerable<Vector3> e)
        {
            if (SelectedUnit == null) return;
            var directions = e.ToArray();
            if (directions.Length != 1)
            {
                Debug.LogWarning($"UnitController::UnitWheeling expects a single direction to be passed but received {directions.Length}");
                return;
            }

            SelectedUnit.Wheel(directions[0], isRunning: false);
        }

        public void StopWheeling()
        {
            if (SelectedUnit != null)
                SelectedUnit.StopWheel();
        }

        public void ManualMove(IEnumerable<Vector3> e)
        {
            if (SelectedUnit == null) return;
            var directions = e.ToArray();
            //currently only care about first element
            SelectedUnit.ManualMove(directions[0], isRunning: false);
        }

        public void StopManualMove()
        {
            if (SelectedUnit != null)
                SelectedUnit.StopManualMove();
        }

        private Unit GetSelectedUnit(GameObject unitsCollection, int id)
        {
            var units = unitsCollection.GetComponentsInChildren(typeof(Unit), includeInactive: true);

            if (units == null)
            {
                Debug.LogWarning("units component returned null!");
                return null;
            }

            foreach (Unit unit in units)
            {
                if (unit.ID == id)
                {
                    return unit;
                }
            }

            return null;
        }
    }
}
