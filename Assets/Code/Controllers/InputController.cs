using System;
using System.Collections.Generic;
using PrimoVictoria.Core.Events;
using PrimoVictoria.Utilities;
using UnityEngine;

namespace PrimoVictoria.Controllers
{
    /// <summary>
    /// Controller class that handles all player input and raises them up through events that subscribing controllers can care about
    /// </summary>
    public class InputController : MonoBehaviour
    {
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
            HandleCameraKeys();
            if (HandleWheeling()) return;
            if (HandleManualMovement()) return;
        }

        /// <summary>
        /// Monitors and watches for keyboard and controller inputs
        /// </summary>
        private void HandleCameraKeys()
        {
            if (Input.GetButtonUp("DeveloperMode"))
            {
                EventManager.Publish(PrimoEvents.UserInterfaceChange, new UserInterfaceArgs(UserInterfaceArgs.UserInterfaceCommand.ToggleDeveloperMode));
            }

            var xAxis = Input.GetAxis("Horizontal");
            var zAxis = Input.GetAxis("Vertical");
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            var mouse1 = Input.GetKey(KeyCode.Mouse1);
            var mouse2 = Input.GetKey(KeyCode.Mouse2);
            var rotation = Input.GetAxis("Rotation");
            
            var cameraArgs = new CameraEventArgs(new Vector2(xAxis, zAxis),rotation, new CameraEventArgs.MouseData(mouse1, mouse2, scroll));
            EventManager.Publish(PrimoEvents.CameraMove,cameraArgs);
        }

        /// <summary>
        /// Checks for any input indicating a wheel should happen and returns TRUE if so
        /// </summary>
        /// <returns></returns>
        private bool HandleWheeling()
        {
            if (Input.GetButton(StaticResources.WHEEL_LEFT))
            {
                EventManager.Publish(PrimoEvents.UnitWheeling, new MovementArgs(new List<Vector3>()
                {
                    Vector3.left
                }));

                return true;
            }

            if (Input.GetButtonUp(StaticResources.WHEEL_LEFT))
            {
                EventManager.Publish(PrimoEvents.StopWheeling, PrimoBaseEventArgs.Empty());
                return true;
            }

            if (Input.GetButton(StaticResources.WHEEL_RIGHT))
            {
                EventManager.Publish(PrimoEvents.UnitWheeling, new MovementArgs(new List<Vector3>()
                {
                    Vector3.right
                }));
                return true;
            }
            
            if (Input.GetButtonUp(StaticResources.WHEEL_RIGHT))
            {
                EventManager.Publish(PrimoEvents.StopWheeling, PrimoBaseEventArgs.Empty());
                return true;
            }

            return false;
        }

        private bool HandleManualMovement()
        {
            var xAxis = Input.GetAxis(StaticResources.MOVE_UNIT_RIGHT_LEFT);
            var zAxis = Input.GetAxis(StaticResources.MOVE_UNIT_UP_DOWN);

            if (Input.GetButtonUp(StaticResources.MOVE_UNIT_RIGHT_LEFT) || Input.GetButtonUp(StaticResources.MOVE_UNIT_UP_DOWN))
            {
                EventManager.Publish(PrimoEvents.StopManualMove, PrimoBaseEventArgs.Empty());
                return true;
            }

            if (zAxis > 0)
            {
                EventManager.Publish(PrimoEvents.UnitManualMove, new MovementArgs(new List<Vector3>()
                {
                    Vector3.forward
                }));
                return true;
            }

            if (zAxis < 0)
            {
                EventManager.Publish(PrimoEvents.UnitManualMove, new MovementArgs(new List<Vector3>()
                {
                    Vector3.back
                }));
                return true;
            }

            if (xAxis > 0)
            {
                EventManager.Publish(PrimoEvents.UnitManualMove, new MovementArgs(new List<Vector3>()
                {
                    Vector3.right
                }));
                return true;
            }

            if (xAxis < 0)
            {
                EventManager.Publish(PrimoEvents.UnitManualMove, new MovementArgs(new List<Vector3>()
                {
                    Vector3.left
                }));
                return true;
            }

            return false;
        }
    }
}
