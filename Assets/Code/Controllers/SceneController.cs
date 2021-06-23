using System;
using PrimoVictoria.Core.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using PrimoVictoria.Utilities;

namespace PrimoVictoria.Controllers
{ 
/// <summary>
/// Centralized controller class that is the main architect of the entire game
/// </summary>
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance = null;  //allows us to access the instance of this object from any other script

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                //destroy this.  There can only ever be one instance of a game manager (Scene Controller)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);  //sets this to not be destroyed when loading scenes

            InitGame();
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
            
            EventManager.Publish(PrimoEvents.InitializeGame, EventArgs.Empty);
        }
    }
}
