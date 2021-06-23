using System;
using System.Collections.Generic;
using System.Text;
using PrimoVictoria.Core;
using PrimoVictoria.Core.Events;
using PrimoVictoria.Models;
using TMPro;
using UnityEngine;

namespace PrimoVictoria.Controllers
{
    /// <summary>
    /// Controller that manages User Interface elements
    /// </summary>
    public class UIController : MonoBehaviour
    {
        public float DebugDurationSeconds = 5.0f;
        public float WheelGizmoSeconds = 0.10f;
        public int DebugLineHeight = 5;
        public List<Vector3> SelectedUnitDestinations; //will display as debug lines

        public Guid Id;

        private GameObject _devConsole;
        private TextMeshProUGUI _devConsoleText;
        private Vector3 _mouseClickPosition;
        private Vector3 _selectedUnitLocationVector;

        private bool IsDevConsoleVisible
        {
            get => _devConsole.activeInHierarchy;
            set => _devConsole.SetActive(value);
        }

        private string selectedUnitLocation => _selectedUnitLocationVector == Vector3.zero ? 
            "Selected Cmd Stand Location: <none>" : 
            $"Selected Cmd Stand Location: {_selectedUnitLocationVector}";

        private string currentMouseClickPosition => $"Current Mouse Click Position:  {_mouseClickPosition}";

        private void Start()
        {
            InitializeController();
            Id = Guid.NewGuid();
            SelectedUnitDestinations = new List<Vector3>();
        }

        private void Update()
        {
            HandleInput();
            WriteOutput();
        }

        private void InitializeController()
        {
            _devConsole = transform.Find("DevConsole").gameObject;
            _devConsoleText = _devConsole.GetComponentInChildren<TextMeshProUGUI>();

            EventManager.Subscribe<MouseClickEventArgs>(PrimoEvents.MouseOverGameBoard, MouseOverGameBoard);
            EventManager.Subscribe<StandLocationArgs>(PrimoEvents.SelectedUnitLocationChanged, SelectedUnitLocationChanged);
            EventManager.Subscribe<UserInterfaceArgs>(PrimoEvents.UserInterfaceChange, UserInterfaceChange);
        }

        /// <summary>
        /// Monitors and watches for keyboard and controller inputs
        /// </summary>
        private void HandleInput()
        {
            if (Input.GetButtonUp("DeveloperMode")) IsDevConsoleVisible = !IsDevConsoleVisible;
        }

        private void WriteOutput()
        {
            if (!IsDevConsoleVisible) return;
            var output = new StringBuilder();
            output.AppendLine(selectedUnitLocation);
            output.AppendLine(currentMouseClickPosition);

            _devConsoleText.text = output.ToString();
        }

        private void MouseOverGameBoard(MouseClickEventArgs e)
        {
            _mouseClickPosition = e.WorldPosition;
        }

        private void SelectedUnitLocationChanged(StandLocationArgs e)
        {
            _selectedUnitLocationVector = e.StandLocation;
        }

        private void UserInterfaceChange(UserInterfaceArgs e)
        {
            if (!IsDevConsoleVisible) return;

            switch (e.Command)
            {
                case UserInterfaceArgs.UserInterfaceCommand.DrawWheelPoints:
                    DrawWheelPoints(e.Unit);
                    break;
                case UserInterfaceArgs.UserInterfaceCommand.DrawUnitDestination:
                    DrawUnitLocation(e.Positions);
                    break;
            }
        }

        private void DrawWheelPoints(Unit unit)
        {
            //make sure gizmos button is pressed in game view or you wont see this
            //the actual pivots will be blue
            //the other pivots will be red
            Debug.DrawRay(unit.GetUnitWheelPoint(Unit.WheelPointIndices.Left_UpperLeft), Vector3.up, Color.blue, WheelGizmoSeconds);
            Debug.DrawRay(unit.GetUnitWheelPoint(Unit.WheelPointIndices.Right_UpperRight), Vector3.up, Color.blue, WheelGizmoSeconds);
        }

        /// <summary>
        /// Draws debug lines to the locations
        /// </summary>
        /// <param name="standLocations"></param>
        private void DrawUnitLocation(List<Vector3> standLocations)
        {
            //developer note: make sure the gizmos button is pressed in the gameview or you wont see this
            var firstItem = true;
            foreach (var point in standLocations)
            {
                var color = Color.red;
                if (firstItem)
                {
                    color = Color.white;
                    firstItem = false;
                }
                var up = transform.TransformDirection(Vector3.up) * DebugLineHeight;
                Debug.DrawRay(point, up, color, DebugDurationSeconds);
            }
        }
    }
}
