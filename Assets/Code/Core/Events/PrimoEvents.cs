using System;

namespace PrimoVictoria.Core.Events
{
    /// <summary>
    /// Valid events that can be published and subscribed to
    /// </summary>
    public enum PrimoEvents
    {
        [PrimoEventsAttribute(typeof(PrimoBaseEventArgs))]
        StopWheeling,
        [PrimoEventsAttribute(typeof(PrimoBaseEventArgs))]
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
        [PrimoEventsAttribute(typeof(PrimoBaseEventArgs))]
        InitializeGame,
        [PrimoEvents(typeof(CameraEventArgs))]
        CameraMove
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