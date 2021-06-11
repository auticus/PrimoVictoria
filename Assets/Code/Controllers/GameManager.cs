﻿using System;
using System.Collections.Generic;
using PrimoVictoria.Assets.Code.Controllers;
using PrimoVictoria.Assets.Code.Models;
using PrimoVictoria.Assets.Code.Models.Parameters;
using PrimoVictoria.Assets.Code.Models.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using PrimoVictoria.Models;
using PrimoVictoria.DataModels;
using PrimoVictoria.UI.Cameras;

namespace PrimoVictoria.Controllers
{ 
/// <summary>
/// Centralized controller class that is the main architect of the entire game
/// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;  //allows us to access the instance of this object from any other script
        public Projectors Projectors; //collection of all Projectors
        
        public const string MESH_DECORATOR_TAG = "UnitMeshDecorator";
        public const string SELECT_BUTTON = "Input1"; //the name of the control set in bindings
        public const string EXECUTE_BUTTON = "Input2";
        public const string WHEEL_LEFT = "WheelUnitLeft";
        public const string WHEEL_RIGHT = "WheelUnitRight";
        public const string MOVE_UNIT_UP_DOWN = "MoveUnitUpDown";
        public const string MOVE_UNIT_RIGHT_LEFT = "MoveUnitRightLeft";

        [SerializeField] List<UnitData> Faction_0_Units;

        private const string PRELOAD_SCENE = "Preload";
        private const string SANDBOX_SCENE = "Sandbox";
        private const string UNITS_GAMEOBJECT = "Units";
        private UIController _uiController; //the reference to the UIController (for things like the dev console etc)

        private Unit _selectedUnit;
        public Unit SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                if (_selectedUnit != null && value != null && _selectedUnit.ID == value.ID)
                    return;

                if (_selectedUnit != null)
                {
                    _selectedUnit.Unselect();
                    _selectedUnit.OnLocationChanged -= HandleSelectedStandLocation;
                }

                _selectedUnit = value;

                if (_selectedUnit != null)
                {
                    _selectedUnit.Select(Projectors,
                        isFriendly: true); //todo: tell if friend or not and not hardcode it to always be friend

                    _selectedUnit.OnLocationChanged += HandleSelectedStandLocation;
                }
                else
                {
                    _uiController.SelectedUnitLocation = Vector3.zero;
                }
            }
        }

        private List<Vector3> _selectedUnitDestinations;
        public List<Vector3> SelectedUnitDestinations
        {
            get => _selectedUnitDestinations;
            set
            {
                _selectedUnitDestinations = value;
                _uiController.SetNewUnitLocation(_selectedUnitDestinations);
            }
        }

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                //destroy this.  There can only ever be one instance of a game manager
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);  //sets this to not be destroyed when loading scenes

            InitGame();
        }

        private void Update()
        {
            HandleInputs();
        }
    
        private void InitGame()
        {
            //initialize the game here
            var scene = SceneManager.GetActiveScene();

            //if you are running through the unity designer and have your sandbox or whatever running, the preload scene isn't running and this is coming from the sandbox's game manager object
            if (scene.name == PRELOAD_SCENE)
            {               
                SceneManager.LoadScene(SANDBOX_SCENE);
            }

            SubscribeToGameEvents();
            SetGameObjectReferences();

            var unitsCollection = GameObject.Find(UNITS_GAMEOBJECT);

            //add my Units collection if it doesn't already exist
            if (unitsCollection == null)
            {
                unitsCollection = new GameObject(UNITS_GAMEOBJECT);
            }

            LoadUnits(unitsCollection);
        }

        private void SubscribeToGameEvents()
        {
            var camera = GameObject.Find("/Main Camera");
            if (!camera)
            {
                Debug.LogError("Main camera was not found!");
                return;
            }
            var rayCaster = camera.GetComponent<CameraRaycaster>();
            rayCaster.OnMouseOverGamePiece += MouseOverGamePiece;
            rayCaster.OnMouseOverTerrain += MouseOverTerrain;
            rayCaster.OnMouseClickOverGameBoard += MouseClickOverGameBoard;
        }

        private void SetGameObjectReferences()
        {
            _uiController = FindObjectOfType<UIController>();
            if (_uiController == null)
            {
                Debug.LogWarning("The game requires that the UIController component exist on the PrimoUI GameObject");
            }
        }

        private void LoadUnits(GameObject unitsCollection)
        {
            //todo: this whole thing is hardcoded and is only for dev purposes, this will need redefined after development to not hardcode the unit types
            //todo: a loading screen of some kind will populate what units are present, for right now this is just loaded with a test unit

            //IMPORTANT
            //the Game Manager instance in the editor will have had units added to it (which is why there is no code here adding any but we are referencing them)
            var location = new Vector3(50, 0, 50);

            //rotation is based on the Y axis
            var rotation = new Vector3(0, 45, 0);
            var unit =UnitFactory.BuildUnit(unitsCollection, "Men at Arms", 1, Faction_0_Units[0], location, rotation, stands: 1, standCountWidth: 1);
            unit.ToggleDiagnostic(true);

            /*
            //UNIT 2
            location = new Vector3(90.0f, 0.4f, 80.7f);
            rotation = new Vector3(0,0,0);
            UnitFactory.BuildUnit(unitsCollection, "Men At Arms 2", 2, Faction_0_Units[0], location, rotation, stands: 1, standCountWidth: 1);
            */
        }

        private void HandleSelectedStandLocation(object sender, StandLocationArgs e)
        {
            _uiController.SelectedUnitLocation = e.StandLocation;
        }

        /// <summary>
        /// This is fired off whenever the mouse moves over a game piece, and will also return if a button was clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseOverGamePiece(object sender, MouseClickGamePieceEventArgs e)
        {
            if (e.Button == MouseClickEventArgs.MouseButton.Input1)
            {
                //left-clicking on a unit will select that unit
                SelectUnitMeshes(e.UnitID);
            }
            if (e.Button == MouseClickEventArgs.MouseButton.Input2)
            {
                //right-clicking on a unit is the same as deselecting that unit
                SelectedUnit = null;
            }
        }

        private void MouseOverTerrain(object sender, Vector3 terrainCoordinates)
        {
            throw new NotImplementedException("Mouse Over Terrain is not currently implemented");
        }

        /// <summary>
        /// this will be captured whenever a mouse click has hit the game board itself and not one of the pieces or terrain structures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseClickOverGameBoard (object sender, MouseClickEventArgs e)
        {
            if (e.Button == MouseClickEventArgs.MouseButton.Input1)
            {
                SelectedUnit = null;
            }
            if (e.Button == MouseClickEventArgs.MouseButton.Input2 && SelectedUnit != null)
            {
                SelectedUnit.Move(e.WorldPosition, isRunning: false);
            }

            _uiController.MouseClickPosition = e.WorldPosition;
        }

        private void SelectUnitMeshes(int unitID)
        {
            var unitsCollection = GameObject.Find(UNITS_GAMEOBJECT);

            if (unitsCollection == null)
                Debug.LogWarning("unitsCollection was NULL!");

            var units = unitsCollection.GetComponentsInChildren(typeof(Unit), includeInactive: true);

            if (units == null)
                Debug.LogWarning("units component returned null!");

            foreach (Unit unit in units)
            {
                if (unit.ID == unitID)
                {
                    SelectedUnit = unit;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles Keyboard / controller inputs
        /// </summary>
        private void HandleInputs()
        {
            if (SelectedUnit != null)
                HandleSelectedUnitInputs();
        }

        private void HandleSelectedUnitInputs()
        {
            if (HandleSelectedUnitWheeling()) return;
            if (HandleSelectedUnitMovement()) return;
        }

        private bool HandleSelectedUnitWheeling()
        {
            if (Input.GetButton(WHEEL_LEFT))
            {
                SelectedUnit.Wheel(Vector3.left, isRunning: false);
                _uiController.DrawWheelPoints(SelectedUnit);
                return true;
            }

            if (Input.GetButtonUp(WHEEL_LEFT))
            {
                SelectedUnit.StopWheel();
                return true;
            }

            if (Input.GetButton(WHEEL_RIGHT))
            {
                SelectedUnit.Wheel(Vector3.right, isRunning: false);
                _uiController.DrawWheelPoints(SelectedUnit);
                return true;
            }

            if (Input.GetButtonUp(WHEEL_RIGHT))
            {
                SelectedUnit.StopWheel();
                return true;
            }

            return false;
        }

        private bool HandleSelectedUnitMovement()
        {
            var xAxis = Input.GetAxis(MOVE_UNIT_RIGHT_LEFT);
            var zAxis = Input.GetAxis(MOVE_UNIT_UP_DOWN);

            if (Input.GetButtonUp(MOVE_UNIT_RIGHT_LEFT) || Input.GetButtonUp(MOVE_UNIT_UP_DOWN))
            {
                SelectedUnit.StopManualMove();
                return true;
            }

            if (zAxis > 0)
            {
                SelectedUnit.ManualMove(Vector3.forward, isRunning: false);
                return true;
            }

            if (zAxis < 0)
            {
                SelectedUnit.ManualMove(Vector3.forward * -1, isRunning: false);
                return true;
            }

            if (xAxis > 0)
            {
                SelectedUnit.ManualMove(Vector3.right, isRunning: false);
                return true;
            }

            if (xAxis < 0)
            {
                SelectedUnit.ManualMove(Vector3.right * -1, isRunning: false);
                return true;
            }

            return false;
        }
    }
}
