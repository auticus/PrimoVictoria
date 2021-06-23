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
            if (HandleWheeling()) return;
            if (HandleManualMovement()) return;
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
                EventManager.Publish(PrimoEvents.StopWheeling, EventArgs.Empty);
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
                EventManager.Publish(PrimoEvents.StopWheeling, EventArgs.Empty);
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
                EventManager.Publish(PrimoEvents.StopManualMove, EventArgs.Empty);
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
