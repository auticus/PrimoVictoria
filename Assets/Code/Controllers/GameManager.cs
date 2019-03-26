using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PrimoVictoria.Models;
using PrimoVictoria.DataModels;

namespace PrimoVictoria.Controllers
{ 
/// <summary>
/// Centralized controller class that is the main architect of the entire game
/// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;  //allows us to access the instance of this object from any other script
        [SerializeField] List<UnitData> ArturianUnits;

        private GameObject _selectedUnit;
        private const string PRELOAD_SCENE = "Preload";
        private const string SANDBOX_SCENE = "Sandbox";
        private const string UNITS_GAMEOBJECT = "Units";

        public GameObject SelectedUnit
        {
            get { return _selectedUnit; }
            set { _selectedUnit = value; }
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

            var unitsCollection = GameObject.Find(UNITS_GAMEOBJECT);
            if (unitsCollection == null)
            {
                unitsCollection = new GameObject(UNITS_GAMEOBJECT);
            }

            LoadUnits(unitsCollection);
        }

        private void Update()
        {
        
        }

        private void LoadUnits(GameObject unitsCollection)
        {
            //todo: a loading screen of some kind will populate what units are present, for right now this is just loaded with a test unit
            var exampleUnit = unitsCollection.AddComponent<Unit>();
            var unitSize = new UnitSize
            {
                ModelCount = 10,
                UnitType = UnitSizeType.Unit
            };
            var location = new Vector3(104.81f, 0.02f, 80.736f);
            var rotation = new Vector3(0, 178, 0);

            if (ArturianUnits.Count == 0)
            {
                Debug.LogWarning("Arturian Units has nothing");
                return;
            }

            exampleUnit.Data = ArturianUnits[0];  //obviously we need to not hardcode this, its for setup testing only
            exampleUnit.name = "Example Unit";
            exampleUnit.InitializeUnit(1, unitSize, location, rotation);
        }
    }
}
