using System;
using System.Diagnostics;
using PrimoVictoria.Assets.Code.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using PrimoVictoria.Models;
using PrimoVictoria.Controllers;
using Debug = UnityEngine.Debug;

namespace PrimoVictoria.UI.Cameras
{
    /// <summary>
    /// Primary mouse manager class that is attached to a camera and broadcasts layer changes and mouse clicks over objects
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraRaycaster : MonoBehaviour
    {
        [SerializeField] float DistanceToBackground = 100f;
        [SerializeField] Vector2 CursorHotspot;

        #region Cursors
        [SerializeField] Texture2D NoUnit_Default = null; //no unit is selected, mouse over no units
        [SerializeField] Texture2D NoUnit_Friendly = null; //no unit is selected, mouse over friendly 
        [SerializeField] Texture2D NoUnit_Enemy = null; //no unit is selected, mouse is over an enemy
        [SerializeField] Texture2D Unknown = null;
        #endregion Cursors

        public EventHandler<MouseClickEventArgs> OnMouseOverGameBoard { get; set; }
        public EventHandler<Vector3> OnMouseOverTerrain { get;set; }
        public EventHandler<MouseClickGamePieceEventArgs> OnMouseOverGamePiece { get; set; }

        private const int TERRAIN_LAYER = 8;

        // Update is called once per frame
        private void Update()
        {
            //if we're over a UI object, the only thing we're sending back is the UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //we're over a UI element - return out                
                return;
            }
            else
            {
                PerformRaycasts();
            }
        }

        private void PerformRaycasts()
        {
            //the sequence given here illustrates the layered approach we care about
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (RaycastForUnit(ray))
                return;
            if (RaycastForTerrain(ray))
                return;

            Cursor.SetCursor(NoUnit_Default, CursorHotspot, CursorMode.Auto);
            
            if (!Physics.Raycast(ray, out RaycastHit hitInfo, DistanceToBackground))
                return;

            var button = MouseClickEventArgs.MouseButton.None;

            if (Input.GetButtonDown(StaticResources.SELECT_BUTTON))
            {
                button = MouseClickEventArgs.MouseButton.Input1;
            }
            else if (Input.GetButtonDown(StaticResources.EXECUTE_BUTTON))
            {
                button = MouseClickEventArgs.MouseButton.Input2;
            }

            OnMouseOverGameBoard?.Invoke(this,
                new MouseClickEventArgs()
                {
                    ScreenPosition = Input.mousePosition, 
                    WorldPosition = hitInfo.point,
                    Button = button
                });
        }

        /// <summary>
        /// Searches for a mesh controller (the 3d model) that is hit and returns it, which should contain unit meta data and if found raises OnMouseOverGamePiece event
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        private bool RaycastForUnit(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hitInfo, DistanceToBackground))
                return false;

            var objectHit = hitInfo.collider.gameObject;

            var selectedUnitID = GetUnitID(objectHit);
            if (selectedUnitID == null) return false;

            Cursor.SetCursor(NoUnit_Friendly, CursorHotspot, CursorMode.Auto);
            var args = new MouseClickGamePieceEventArgs
            {
                ScreenPosition = Input.mousePosition,
                WorldPosition = hitInfo.point,
                UnitID = selectedUnitID.Value,
                Button = GetButton()
            };

            OnMouseOverGamePiece?.Invoke(this, args);
            return true;
        }

        private MouseClickEventArgs.MouseButton GetButton()
        {
            if (Input.GetButtonDown(StaticResources.EXECUTE_BUTTON))
            {
                return MouseClickEventArgs.MouseButton.Input2;
            }

            if (Input.GetButtonDown(StaticResources.SELECT_BUTTON))
                return MouseClickEventArgs.MouseButton.Input1;

            return MouseClickEventArgs.MouseButton.None;
        }

        private int? GetUnitID(GameObject objectHit)
        {
            var standHit = objectHit.GetComponent<UnitMiniatureController>();
            if (standHit != null)
            {
                return standHit.ParentUnit.ID;
            }

            return null;
        }

        /// <summary>
        /// Searches for a piece of terrain (house, structure, etc any model that lives on the TERRAIN layer) and returns the vector3 of that object 
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        private bool RaycastForTerrain(Ray ray)
        {
            //todo: return the terrain piece back, not a vector3.  A vector3 is pointless
            var terrainLayerMask = 1 << TERRAIN_LAYER;
            var terrainHit = Physics.Raycast(ray, out var hitInfo, DistanceToBackground, terrainLayerMask);
            if (terrainHit)
            {
                //todo: when units are selected etc, this will need more intelligent, right now we just set it to the default cursor
                Cursor.SetCursor(NoUnit_Default, CursorHotspot, CursorMode.Auto);
                OnMouseOverTerrain?.Invoke(this, hitInfo.point);
                return true;
            }

            return false;
        }
    }
}