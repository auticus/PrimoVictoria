using System;
using System.Collections.Generic;
using System.Text;
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
        public float DebugDurationSeconds = 10.0f;
        public float WheelGizmoSeconds = 0.10f;
        public int DebugLineHeight = 5;
        public Vector3 SelectedUnitLocation;
        public Vector3 MouseClickPosition;
        public List<Vector3> SelectedUnitDestinations; //will display as debug lines

        public Guid Id;

        private GameObject _devConsole;
        private TextMeshProUGUI _devConsoleText;
        private bool _isDevConsoleVisible
        {
            get => _devConsole.activeInHierarchy;
            set => _devConsole.SetActive(value);
        }

        private string _selectedUnitLocation => SelectedUnitLocation == Vector3.zero ? 
            "Selected Cmd Stand Location: <none>" : 
            $"Selected Cmd Stand Location: {SelectedUnitLocation}";

        private string _currentMouseClickPosition => $"Current Mouse Click Position:  {MouseClickPosition}";

        /// <summary>
        /// Draws debug lines to the locations
        /// </summary>
        /// <param name="standLocations"></param>
        public void SetNewUnitLocation(List<Vector3> standLocations)
        {
            if (!_isDevConsoleVisible) return;

            //developer note: make sure the gizmos button is pressed in the gameview or you wont see this
            foreach(var point in standLocations)
            {
                var up = transform.TransformDirection(Vector3.up) * DebugLineHeight;
                Debug.DrawRay(point, up, Color.red, DebugDurationSeconds);
            }
        }

        public void DrawWheelPoints(Unit unit)
        {
            if (!_isDevConsoleVisible) return;

            //make sure gizmos button is pressed in game view or you wont see this
            //the actual pivots will be blue
            //the other pivots will be red
            Debug.DrawRay(unit.GetUnitWheelPoint(Unit.WheelPointIndices.Left_UpperLeft), Vector3.up, Color.blue, WheelGizmoSeconds);
            Debug.DrawRay(unit.GetUnitWheelPoint(Unit.WheelPointIndices.Right_UpperRight), Vector3.up, Color.blue, WheelGizmoSeconds);
        }

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
        }

        /// <summary>
        /// Monitors and watches for keyboard and controller inputs
        /// </summary>
        private void HandleInput()
        {
            if (Input.GetButtonUp("DeveloperMode")) _isDevConsoleVisible = !_isDevConsoleVisible;
        }

        private void WriteOutput()
        {
            if (!_isDevConsoleVisible) return;
            var output = new StringBuilder();
            output.AppendLine(_selectedUnitLocation);
            output.AppendLine(_currentMouseClickPosition);

            _devConsoleText.text = output.ToString();
        }
    }
}
