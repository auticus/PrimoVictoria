using System;
using System.Collections.Generic;
using PrimoVictoria.Assets.Code;
using PrimoVictoria.Assets.Code.Controllers;
using PrimoVictoria.Assets.Code.Models;
using PrimoVictoria.Assets.Code.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using PrimoVictoria.Models;

namespace PrimoVictoria.Controllers
{ 
/// <summary>
/// Centralized controller class that is the main architect of the entire game
/// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;  //allows us to access the instance of this object from any other script
        private InputController _inputController; //all input-related commands go through this gameobject, public because needs set as a gameobject
        private UnitController _unitController;
        private UIController _uiController; //the reference to the UIController (for things like the dev console etc)

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

        public void SetActiveUnit(Unit unit)
        {
            _unitController.SetSelectedUnit(unit);

            if (unit == null)
            {
                _uiController.SelectedUnitLocation = Vector3.zero;
            }
        }

        private void InitGame()
        {
            //initialize the game here
            var scene = SceneManager.GetActiveScene();

            //if you are running through the unity designer and have your sandbox or whatever running, the preload scene isn't running and this is coming from the sandbox's game manager object
            if (scene.name == StaticResources.PRELOAD_SCENE)
            {               
                SceneManager.LoadScene(StaticResources.SANDBOX_SCENE);
            }

            SetGameObjectReferences();
            SubscribeToControllerEvents();
            _unitController.LoadUnits();
        }

        private void SetGameObjectReferences()
        {
            _uiController = FindObjectOfType<UIController>(); //this controller sits on the UI Gameobject, not the gamemanager object
            if (_uiController == null)
            {
                Debug.LogWarning("The game requires that the UIController component exist on the PrimoUI GameObject");
                throw new NullReferenceException("UI Controller is null");
            }

            _inputController = GetComponent<InputController>();
            if (_inputController == null)
            {
                Debug.LogError("The game requires that the GameObject has an InputController component");
                throw new NullReferenceException("Input Controller is null");
            }

            _unitController = GetComponent<UnitController>();
            if (_unitController == null)
            {
                Debug.LogError("The game requires that the GameObject has a UnitController component");
                throw new NullReferenceException("Unit Controller is null");
            }
        }

        #region Event Handlers
        private void SubscribeToControllerEvents()
        {
            //area where we will be subscribing to child controllers and passing their events to other controllers that need or care about them
            _inputController.OnMouseOverGameBoard += MouseOverGameBoard;
            _inputController.OnMouseOverGamePiece += MouseOverGamePiece;
            _inputController.OnWheeling += UnitWheeling;
            _inputController.OnStopWheeling += StopWheeling;
            _inputController.OnStopManualMove += StopUnitManualMove;
            _inputController.OnManualMove += UnitManualMove;

            _unitController.OnSelectedUnitLocationChanged += SelectedUnitLocationChanged;
        }

        private void MouseOverGamePiece(object sender, MouseClickGamePieceEventArgs e)
        {
            switch (e.Button)
            {
                case MouseClickEventArgs.MouseButton.None:
                case MouseClickEventArgs.MouseButton.Input3:
                    //input 3 is currently not used, so if they are clicking on it, treat it like none
                    //todo: make those sweet juicy projectors flow
                    break;
                case MouseClickEventArgs.MouseButton.Input1:
                    //left-clicking on a unit will select that unit
                    _unitController.SelectUnit(e.UnitID);
                    break;
                case MouseClickEventArgs.MouseButton.Input2:
                    //right-clicking on a unit is the same as deselecting that unit
                    SetActiveUnit(null);
                    break;
            }
        }

        private void MouseOverGameBoard(object sender, MouseClickEventArgs e)
        {
            _uiController.MouseClickPosition = e.WorldPosition;
            
            //left-clicking the gameboard unselects any saved units
            //right-clicking the gameboard will attempt to move the selected unit to that point
            if (e.Button == MouseClickEventArgs.MouseButton.Input1)
            {
                SetActiveUnit(null);
            }
            if (e.Button == MouseClickEventArgs.MouseButton.Input2)
            {
                _unitController.MoveSelectedUnitToPoint(e.WorldPosition, isRunning: false);
            }
        }

        private void UnitWheeling(object sender, MovementArgs e)
        {
            _uiController.DrawWheelPoints(_unitController.SelectedUnit);
            _unitController.UnitWheeling(e.Directions);
        }

        private void StopWheeling(object sender, EventArgs e)
        {
            _unitController.StopWheeling();
        }

        private void UnitManualMove(object sender, MovementArgs e)
        {
            _unitController.ManualMove(e.Directions);
        }

        private void StopUnitManualMove(object sender, EventArgs e)
        {
            _unitController.StopManualMove();
        }

        private void SelectedUnitLocationChanged(object sender, StandLocationArgs e)
        {
            _uiController.SelectedUnitLocation = e.StandLocation;
        }
        #endregion
    }
}
