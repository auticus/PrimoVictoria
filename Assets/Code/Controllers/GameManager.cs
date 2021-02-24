using System;
using System.Collections.Generic;
using System.Linq;
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
        public GameObject SelectionProjector = null; //for things that we control (green)
        public GameObject OtherSelectionProjector = null;  //for things that we cannot control (red)

        public const string MESH_DECORATOR_TAG = "UnitMeshDecorator";
        public const string SELECT_BUTTON = "Input1"; //the name of the control set in bindings
        public const string EXECUTE_BUTTON = "Input2"; 

        [SerializeField] List<UnitData> Faction_0_Units;

        private const string PRELOAD_SCENE = "Preload";
        private const string SANDBOX_SCENE = "Sandbox";
        private const string UNITS_GAMEOBJECT = "Units";

        private Unit _selectedUnit;
        public Unit SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                if (_selectedUnit != null && value != null && _selectedUnit.ID == value.ID)
                    return;

                if (_selectedUnit != null)
                    _selectedUnit.Unselect();

                _selectedUnit = value;

                if (_selectedUnit != null)
                    _selectedUnit.Select(SelectionProjector);  //todo: if its not a friendly unit we're selecting will need to pass OtherSelectionProjector
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

            var unitsCollection = GameObject.Find(UNITS_GAMEOBJECT);

            //add my Units collection if it doesn't already exist
            if (unitsCollection == null)
            {
                unitsCollection = new GameObject(UNITS_GAMEOBJECT);
            }

            LoadUnits(unitsCollection);
        }

        private void Update()
        {
        
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

        private void LoadUnits(GameObject unitsCollection)
        {
            //todo: this whole thing is hardcoded and is only for dev purposes, this will need redefined after development to not hardcode the unit types
            //todo: a loading screen of some kind will populate what units are present, for right now this is just loaded with a test unit

            //IMPORTANT
            //the Game Manager instance in the editor will have had units added to it (which is why there is no code here adding any but we are referencing them)
            var exampleUnit = unitsCollection.AddComponent<Unit>();

            var unitSize = new UnitSize
            {
                StandCount = 1,
                UnitType = UnitSizeType.Infantry
            };
            var location = new Vector3(104.81f, 0.02f, 80.736f);
            var rotation = new Vector3(0, 178, 0);
            
            exampleUnit.Data = Faction_0_Units[0];  //obviously we need to not hardcode this, its for setup testing only
            exampleUnit.ID = 1;
            exampleUnit.InitializeUnit(unitsCollection, 1, unitSize, location, rotation);
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
                SelectUnitMeshes(e.GamePieceMesh.UnitID);
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
                SelectedUnit.MoveUnit(e.WorldPosition, isRunning: false);
            }
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
    }
}
