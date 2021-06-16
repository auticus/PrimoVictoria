using System;
using System.Collections.Generic;
using PrimoVictoria.Assets.Code;
using PrimoVictoria.Controllers;
using PrimoVictoria.UI.Cameras;
using UnityEngine;

namespace PrimoVictoria
{
    public class InputController : MonoBehaviour
    {
        public EventHandler<MouseClickEventArgs> OnMouseClickOverGameBoard;
        public EventHandler<MouseClickGamePieceEventArgs> OnMouseClickOverGamePiece;
        public EventHandler<MovementArgs> OnWheeling;
        public EventHandler OnStopWheeling;
        public EventHandler<MovementArgs> OnManualMove;
        public EventHandler OnStopManualMove;

        private void Awake()
        {
            SubscribeToGameEvents();
        }

        private void Update()
        {
            HandleInputs();
        }

        /// <summary>
        /// Handles Keyboard / controller inputs
        /// </summary>
        private void HandleInputs()
        {
            //currently three modes of movement
            //1 - wheeling
            //2 - manual movement
            //3 - point and click with the mouse
            //this covers the first two
            if (HandleWheeling()) return;
            if (HandleManualMovement()) return;
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

        /// <summary>
        /// This is fired off whenever the mouse moves over a game piece, and will also return if a button was clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseOverGamePiece(object sender, MouseClickGamePieceEventArgs e)
        {
            if (e.Button != MouseClickEventArgs.MouseButton.None)
            {
                OnMouseClickOverGamePiece?.Invoke(this, e);
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
        private void MouseClickOverGameBoard(object sender, MouseClickEventArgs e)
        {
            OnMouseClickOverGameBoard?.Invoke(this, e);
        }
        
        /// <summary>
        /// Checks for any input indicating a wheel should happen and returns TRUE if so
        /// </summary>
        /// <returns></returns>
        private bool HandleWheeling()
        {
            if (Input.GetButton(GameManager.WHEEL_LEFT))
            {
                OnWheeling?.Invoke(this, new MovementArgs(new List<Vector3>()
                {
                    Vector3.left
                }));
                return true;
            }

            if (Input.GetButtonUp(GameManager.WHEEL_LEFT))
            {
                OnStopWheeling?.Invoke(this, EventArgs.Empty);
                return true;
            }

            if (Input.GetButton(GameManager.WHEEL_RIGHT))
            {
                OnWheeling?.Invoke(this, new MovementArgs(new List<Vector3>()
                {
                    Vector3.right
                }));
                return true;
            }

            if (Input.GetButtonUp(GameManager.WHEEL_RIGHT))
            {
                OnStopWheeling?.Invoke(this, EventArgs.Empty);
                return true;
            }

            return false;
        }

        private bool HandleManualMovement()
        {
            var xAxis = Input.GetAxis(GameManager.MOVE_UNIT_RIGHT_LEFT);
            var zAxis = Input.GetAxis(GameManager.MOVE_UNIT_UP_DOWN);

            if (Input.GetButtonUp(GameManager.MOVE_UNIT_RIGHT_LEFT) || Input.GetButtonUp(GameManager.MOVE_UNIT_UP_DOWN))
            {
                OnStopManualMove?.Invoke(this, EventArgs.Empty);
                return true;
            }

            if (zAxis > 0)
            {
                OnManualMove?.Invoke(this, new MovementArgs(new List<Vector3>()
                {
                    Vector3.forward
                }));
                return true;
            }

            if (zAxis < 0)
            {
                OnManualMove?.Invoke(this, new MovementArgs(new List<Vector3>()
                {
                    Vector3.back
                }));
                return true;
            }

            if (xAxis > 0)
            {
                OnManualMove?.Invoke(this, new MovementArgs(new List<Vector3>()
                {
                    Vector3.right
                }));
                return true;
            }

            if (xAxis < 0)
            {
                OnManualMove?.Invoke(this, new MovementArgs(new List<Vector3>()
                {
                    Vector3.left
                }));
                return true;
            }

            return false;
        }
    }
}
