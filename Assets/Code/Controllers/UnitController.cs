using System;
using System.Collections.Generic;
using System.Linq;
using PrimoVictoria.Core.Events;
using PrimoVictoria.DataModels;
using PrimoVictoria.Models;
using PrimoVictoria.Utilities;
using UnityEngine;

namespace PrimoVictoria.Controllers
{
    /// <summary>
    /// Controller class that is responsible for issuing orders to the units in the game
    /// </summary>
    public class UnitController : MonoBehaviour
    {
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
                    _selectedUnit.ToggleDiagnostic(false);
                }

                _selectedUnit = value;

                if (_selectedUnit != null)
                {
                    _selectedUnit.SelectFriendly(Projectors); //todo: tell if friend or not and not hardcode it to always be friend
                    _selectedUnit.ToggleDiagnostic(true);
                }
                else
                {
                    EventManager.Publish(PrimoEvents.SelectedUnitLocationChanged, new StandLocationArgs(Vector3.zero));
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
                    _ghostSelectedUnit.GhostSelectFriendly(Projectors);  //todo: remove hardcoded friendly
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

            SubscribeToEventManager();
        }

        private void SubscribeToEventManager()
        {
            EventManager.Subscribe<MouseClickEventArgs>(PrimoEvents.MouseOverGameBoard, MouseOverGameBoard);
            EventManager.Subscribe<MouseClickGamePieceEventArgs>(PrimoEvents.MouseOverGamePiece, MouseOverGamePiece);
            EventManager.Subscribe<MovementArgs>(PrimoEvents.UnitWheeling, UnitWheeling);
            EventManager.Subscribe<PrimoBaseEventArgs>(PrimoEvents.StopWheeling, StopWheeling);
            EventManager.Subscribe<PrimoBaseEventArgs>(PrimoEvents.StopManualMove, StopUnitManualMove);
            EventManager.Subscribe<MovementArgs>(PrimoEvents.UnitManualMove, UnitManualMove);
            EventManager.Subscribe<PrimoBaseEventArgs>(PrimoEvents.InitializeGame, InitializeController);
        }

        private void InitializeController(EventArgs e)
        {
            LoadUnits();
        }

        private void LoadUnits()
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
            unit.ToggleDiagnostic(false);

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
        private void Select(Unit unit)
        {
            SelectedUnit = unit;
        }

        /// <summary>
        /// Selects a Unit based on the ID passed to it and draws projectors underneath it
        /// </summary>
        /// <param name="unitID"></param>
        private void Select(int unitID)
        {
            if (!UnitsCollectionIsValid()) return;
            GhostSelectedUnit = null;
            Select(GetSelectedUnit(UnitsCollection, unitID));
        }

        /// <summary>
        /// Will draw faded projectors underneath a unit (but not actually select it)
        /// </summary>
        /// <param name="unitID"></param>
        private void GhostSelect(int unitID)
        {
            if (!UnitsCollectionIsValid()) return;
            var unit = GetSelectedUnit(UnitsCollection, unitID);

            if (SelectedUnit == null || unit.ID != SelectedUnit.ID)
            {
                GhostSelectedUnit = unit;
            }
        }

        private void ClearGhostSelect()
        {
            GhostSelectedUnit = null;
        }

        private void MoveSelectedUnitToPoint(Vector3 worldPosition, bool isRunning)
        {
            if (SelectedUnit != null)
                SelectedUnit.Move(worldPosition, isRunning: isRunning);
        }

        private void UnitWheeling(IEnumerable<Vector3> e)
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

        private void StopWheeling()
        {
            if (SelectedUnit != null)
                SelectedUnit.StopWheel();
        }

        private void ManualMove(IEnumerable<Vector3> e)
        {
            if (SelectedUnit == null) return;
            var directions = e.ToArray();
            //currently only care about first element
            SelectedUnit.ManualMove(directions[0], isRunning: false);
        }

        private void StopManualMove()
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

        private void MouseOverGamePiece(MouseClickGamePieceEventArgs e)
        {
            switch (e.Button)
            {
                case MouseClickEventArgs.MouseButton.None:
                case MouseClickEventArgs.MouseButton.Input3:
                    //input 3 is currently not used, so if they are clicking on it, treat it like none
                    //we've moused over a game piece, we need to ghost-select it unless it is otherwise already selected
                    GhostSelect(e.UnitID);
                    break;
                case MouseClickEventArgs.MouseButton.Input1:
                    //left-clicking on a unit will select that unit
                    Select(e.UnitID);
                    break;
                case MouseClickEventArgs.MouseButton.Input2:
                    //right-clicking on a unit is the same as deselecting that unit
                    SetActiveUnit(null);
                    break;
            }
        }

        private void MouseOverGameBoard(MouseClickEventArgs e)
        {
            //left-clicking the gameboard unselects any saved units
            //right-clicking the gameboard will attempt to move the selected unit to that point
            //simply moving over the gameboard will deselect any ghost-selected units (units that have ghost icons under them indicating they were moused over)
            if (e.Button == MouseClickEventArgs.MouseButton.Input1)
            {
                SetActiveUnit(null);
            }
            if (e.Button == MouseClickEventArgs.MouseButton.Input2)
            {
                MoveSelectedUnitToPoint(e.WorldPosition, isRunning: false);
            }

            ClearGhostSelect();
        }

        private void SetActiveUnit(Unit unit)
        {
            Select(unit);

            if (unit == null)
            {
                EventManager.Publish(PrimoEvents.SelectedUnitLocationChanged, new StandLocationArgs(Vector3.zero));
            }
        }

        private void UnitWheeling(MovementArgs e)
        {
            EventManager.Publish(PrimoEvents.UserInterfaceChange, new UserInterfaceArgs(SelectedUnit, UserInterfaceArgs.UserInterfaceCommand.DrawWheelPoints));
            UnitWheeling(e.Directions);
        }

        private void StopWheeling(EventArgs e)
        {
            StopWheeling();
        }

        private void UnitManualMove(MovementArgs e)
        {
            ManualMove(e.Directions);
        }

        private void StopUnitManualMove(EventArgs e)
        {
            StopManualMove();
        }
    }
}
