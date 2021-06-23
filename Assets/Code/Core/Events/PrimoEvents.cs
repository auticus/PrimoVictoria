using System;

namespace PrimoVictoria.Core.Events
{
    /// <summary>
    /// Events that take in no arguments
    /// </summary>
    public enum PrimoEvents
    {
        [PrimoEventsAttribute(typeof(EventArgs))]
        StopWheeling,
        [PrimoEventsAttribute(typeof(EventArgs))]
        StopManualMove,
        [PrimoEventsAttribute(typeof(MovementArgs))]
        UnitWheeling,
        [PrimoEventsAttribute(typeof(MovementArgs))]
        UnitManualMove,
        [PrimoEventsAttribute(typeof(MouseClickEventArgs))]
        MouseOverGameBoard,
        [PrimoEventsAttribute(typeof(MouseClickGamePieceEventArgs))]
        MouseOverGamePiece,
        [PrimoEventsAttribute(typeof(StandLocationArgs))]
        SelectedUnitLocationChanged,
        [PrimoEventsAttribute(typeof(UserInterfaceArgs))]
        UserInterfaceChange,
        [PrimoEventsAttribute(typeof(EventArgs))]
        InitializeGame
    }

    /// <summary>
    /// Which attribute is valid for this event
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PrimoEventsAttribute : Attribute
    {
        public PrimoEventsAttribute(Type argsType)
        {
            ArgsType = argsType;
        }

        public Type ArgsType { get; }
    }
}