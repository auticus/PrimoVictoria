using UnityEngine;

namespace PrimoVictoria.Core.Events
{
    public class MouseClickGamePieceEventArgs : MouseClickEventArgs
    {
        public int UnitID { get; }

        public MouseClickGamePieceEventArgs(int unitID, Vector3 screenPos, Vector3 worldPos, MouseButton button) : base(screenPos, worldPos, button)
        {
            UnitID = unitID;
        }
    }
}
