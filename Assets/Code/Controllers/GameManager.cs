using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrimoVictoria.Controllers
{ 
/// <summary>
/// Centralized controller class that is the main architect of the entire game
/// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;  //allows us to access the instance of this object from any other script

        private GameObject _selectedUnit;
        private const string PRELOAD_SCENE = "Preload";
        private const string SANDBOX_SCENE = "Sandbox";

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

            if (scene.name == PRELOAD_SCENE)
                SceneManager.LoadScene(SANDBOX_SCENE);
        }

        private void Update()
        {
        
        }
    }
}
