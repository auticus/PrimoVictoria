using System;
using UnityEngine;

namespace PrimoVictoria
{
    public sealed class MouseClickEventArgs : EventArgs
    {
        public enum MouseButton
        {
            Input1,
            Input2,
            Input3
        }

        public Vector3 MousePosition { get; set; }
        public MouseButton Button { get; set; }
    }
}
