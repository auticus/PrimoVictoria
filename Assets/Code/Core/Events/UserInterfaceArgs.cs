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
            DrawUnitDestination
        }

        public UserInterfaceCommand Command { get; }
        public Unit Unit { get; }
        public List<Vector3> Positions;

        //not recording these as commands currently
        public UserInterfaceArgs(Unit unit, UserInterfaceCommand command)
        {
            Command = command;
            Unit = unit;
            Positions = new List<Vector3>();
        }

        //not recording these as commands currently
        public UserInterfaceArgs(Unit unit, UserInterfaceCommand command, List<Vector3> positions):this(unit, command)
        {
            Positions = positions;
        }
    }
}
