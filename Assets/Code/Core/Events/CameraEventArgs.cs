using UnityEngine;

namespace PrimoVictoria.Core.Events
{
    public class CameraEventArgs : PrimoBaseEventArgs
    {
        public struct MouseData
        {
            public bool Button1;
            public bool Button2;
            public float Scroll;

            public MouseData(bool button1, bool button2,  float scroll)
            {
                Button1 = button1;
                Button2 = button2;
                Scroll = scroll;
            }
        }

        public Vector2 Axis { get; }
        public float RotationAxis { get; }
        public MouseData Mouse { get; }

        public CameraEventArgs(Vector2 axis, float rotationAxis)
        {
            Axis = axis;
            RotationAxis = rotationAxis;
            Mouse = new MouseData();
        }

        public CameraEventArgs(Vector2 axis, float rotationAxis, MouseData mouse)
        {
            Axis = axis;
            RotationAxis = rotationAxis;
            Mouse = mouse;
        }
    }
}
