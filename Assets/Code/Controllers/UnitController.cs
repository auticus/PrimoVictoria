using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PrimoVictoria.Assets.Code.Models;
using PrimoVictoria.Assets.Code.Utilities;
using PrimoVictoria.Code.Utilities;
using PrimoVictoria.DataModels;
using PrimoVictoria.Models;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PrimoVictoria.Controllers
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
        /// <summary>
        /// The current actively selected unit
        /// </summary>
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
                    _selectedUnit.Select(Projectors, isFriendly: true); //todo: tell if friend or not and not hardcode it to always be friend

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

        private Unit _ghostSelectedUnit;
        /// <summary>
        /// Unit that has a ghosted selection series underneath it
        /// </summary>
        public Unit GhostSelectedUnit
        {
            get => _ghostSelectedUnit;
            private set
            {
                var nullUnit = value == null;
                if (_ghostSelectedUnit != null && value != null && _ghostSelectedUnit.ID == value.ID)
                {
                    return;
                }
                if (_ghostSelectedUnit != null)
                {
                    _ghostSelectedUnit.Unselect();
                }

                _ghostSelectedUnit = value;

                if (_ghostSelectedUnit != null)
                {
                    _ghostSelectedUnit.GhostSelect(Projectors, isFriendly: true);  //todo: remove hardcoded friendly
                }
            }
        }

        void Awake()
        {
            UnitsCollection = GameObject.Find(UNITS_GAMEOBJECT);
            if (UnitsCollection == null)
            {
                UnitsCollection = new GameObject(UNITS_GAMEOBJECT) {layer = StaticResources.MINIATURES_LAYER};
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

        /// <summary>
        /// Selects the Unit passed in and draws projectors underneath it
        /// </summary>
        /// <param name="unit"></param>
        public void Select(Unit unit)
        {
            SelectedUnit = unit;
        }

        /// <summary>
        /// Selects a Unit based on the ID passed to it and draws projectors underneath it
        /// </summary>
        /// <param name="unitID"></param>
        public void Select(int unitID)
        {
            if (!UnitsCollectionIsValid()) return;
            GhostSelectedUnit = null;
            Select(GetSelectedUnit(UnitsCollection, unitID));
        }

        /// <summary>
        /// Will draw faded projectors underneath a unit (but not actually select it)
        /// </summary>
        /// <param name="unitID"></param>
        public void GhostSelect(int unitID)
        {
            if (!UnitsCollectionIsValid()) return;
            var unit = GetSelectedUnit(UnitsCollection, unitID);

            if (SelectedUnit == null || unit.ID != SelectedUnit.ID)
            {
                GhostSelectedUnit = unit;
            }
        }

        public void ClearGhostSelect()
        {
            GhostSelectedUnit = null;
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

        private bool UnitsCollectionIsValid()
        {
            if (UnitsCollection == null)
            {
                Debug.LogWarning("UnitController::UnitsCollection was NULL!");
                SelectedUnit = null;
                return false;
            }

            return true;
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
