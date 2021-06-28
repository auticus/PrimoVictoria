using UnityEngine;

namespace PrimoVictoria.Core.Events
{
    public class MouseClickEventArgs : PrimoBaseEventArgs
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
        public Vector3 ScreenPosition { get; }

        /// <summary>
        /// WORLD position of the mouse
        /// </summary>
        public Vector3 WorldPosition { get; }

        public MouseButton Button { get; }

        public MouseClickEventArgs( Vector3 screenPos, Vector3 worldPos, MouseButton button)
        {
            ScreenPosition = screenPos;
            WorldPosition = worldPos;
            Button = button;
        }
    }
}
