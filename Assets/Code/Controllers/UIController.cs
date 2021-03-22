using System;
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
        public Vector3 SelectedUnitLocation;
        public Vector3[] SelectedUnitSockets;

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

        private string _selectedUnitSockets
        {
            get
            {
                if (SelectedUnitSockets == null || SelectedUnitSockets.Length == 0)
                    return "Selected Cmd Stand Sockets: <none>";

                var sb = new StringBuilder();
                sb.Append("Selected Cmd Stand Sockets: ");
                foreach (var socket in SelectedUnitSockets)
                {
                    sb.Append($"{socket}::");
                }

                return sb.ToString();
            }
        }

        private void Start()
        {
            InitializeController();
            Id = Guid.NewGuid();
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
            output.AppendLine(_selectedUnitSockets);

            _devConsoleText.text = output.ToString();
        }
    }
}
