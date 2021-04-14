﻿using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace PrimoVictoria.Assets.Code.Controllers
{
    /// <summary>
    /// Controller that manages User Interface elements
    /// </summary>
    public class UIController : MonoBehaviour
    {
        public float DebugDurationSeconds = 10.0f;
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
            $"Selected Cmd Stand Location: {SelectedUnitLocation.ToString()}";

        private string _currentMouseClickPosition => $"Current Mouse Click Position:  {MouseClickPosition.ToString()}";

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
