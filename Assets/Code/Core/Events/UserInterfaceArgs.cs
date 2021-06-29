using System.Collections.Generic;
using PrimoVictoria.Models;
using UnityEngine;

namespace PrimoVictoria.Core.Events
{
    public class UserInterfaceArgs : PrimoBaseEventArgs
    {
        public enum UserInterfaceCommand
        {
            DrawWheelPoints,
            DrawUnitDestination,
            ToggleDeveloperMode
        }

        public UserInterfaceCommand Command { get; }
        public Unit Unit { get; }
        public List<Vector3> Positions;

        public UserInterfaceArgs(UserInterfaceCommand command)
        {
            Command = command;
        }

        public UserInterfaceArgs(Unit unit, UserInterfaceCommand command):this(command)
        {
            Unit = unit;
            Positions = new List<Vector3>();
        }

        public UserInterfaceArgs(Unit unit, UserInterfaceCommand command, List<Vector3> positions):this(unit, command)
        {
            Positions = positions;
        }
    }
}
