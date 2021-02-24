using System;
using UnityEngine;
using PrimoVictoria.Models;
using PrimoVictoria.Controllers;

namespace PrimoVictoria
{
    public class MouseClickEventArgs : EventArgs
    {
        public enum MouseButton
        {
            None = 0,
            /// <summary>
            /// Default Left-Click
            /// </summary>
            Input1,
            /// <summary>
            /// Default Right-Click
            /// </summary>
            Input2,
            /// <summary>
            /// Default Middle-Click
            /// </summary>
            Input3
        }

        /// <summary>
        /// SCREEN position of the mouse
        /// </summary>
        public Vector3 ScreenPosition { get; set; }

        /// <summary>
        /// WORLD position of the mouse
        /// </summary>
        public Vector3 WorldPosition { get; set; }

        public MouseButton Button { get; set; }
    }

    public class MouseClickGamePieceEventArgs : MouseClickEventArgs
    {
        public UnitMeshController GamePieceMesh { get; set; }
    }
}
