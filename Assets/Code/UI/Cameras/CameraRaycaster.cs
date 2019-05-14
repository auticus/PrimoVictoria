using System;
using UnityEngine;
using UnityEngine.EventSystems;
using PrimoVictoria.Models;
using PrimoVictoria.Controllers;

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
        [SerializeField] Texture2D NoUnit_Default = null;
        [SerializeField] Texture2D NoUnit_Friendly = null;
        [SerializeField] Texture2D NoUnit_Enemy = null;
        [SerializeField] Texture2D Unknown = null;
        #endregion Cursors

        public EventHandler<MouseClickEventArgs> OnMouseClickOverGameBoard { get; set; }
        public EventHandler<Vector3> OnMouseOverTerrain { get;set; }
        public EventHandler<UnitMeshController> OnMouseOverGamePiece { get; set; }

        private Camera view;
        private const int TERRAIN_LAYER = 8;
        

        // Start is called before the first frame update
        private void Start()
        {
            view = Camera.main;
        }

        // Update is called once per frame
        private void Update()
        {
            //if we're over a UI object, the only thing we're sending back is the UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
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

            if (Input.GetButtonDown(GameManager.SELECT_BUTTON))
            {
                OnMouseClickOverGameBoard?.Invoke(this, new MouseClickEventArgs() { MousePosition=Input.mousePosition, Button = MouseClickEventArgs.MouseButton.Input1 });
            }
        }

        /// <summary>
        /// Searches for a mesh controller (the 3d model) that is hit and returns it, which should contain unit meta data
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        private bool RaycastForUnit(Ray ray)
        {
            RaycastHit hitInfo;

            if (!Physics.Raycast(ray, out hitInfo, DistanceToBackground))
                return false;

            var objectHit = hitInfo.collider.gameObject;

            var unitHit = objectHit.GetComponent<UnitMeshController>();
            if (unitHit != null)
            {
                //todo: when implementing selected units and you want to charge, etc, this will matter and you will need to change the cursor more intelligently
                //for right now every unit we just use the same cursor for
                Cursor.SetCursor(NoUnit_Friendly, CursorHotspot, CursorMode.Auto);
                OnMouseOverGamePiece?.Invoke(this, unitHit);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Searches for a piece of terrain (house, structure, etc any model that lives on the TERRAIN layer) and returns the vector3 of that object 
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        private bool RaycastForTerrain(Ray ray)
        {
            //todo: return the terrain piece back, not a vector3.  A vector3 is pointless
            RaycastHit hitInfo;
            var terrainLayerMask = 1 << TERRAIN_LAYER;
            var terrainHit = Physics.Raycast(ray, out hitInfo, DistanceToBackground, terrainLayerMask);
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