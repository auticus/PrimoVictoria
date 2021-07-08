namespace PrimoVictoria.Core.Events
{
    public class KeyEventArgs : PrimoBaseEventArgs
    {
        public enum PrimoKeyState
        {
            Up,
            Down,
            Pressed
        }

        public PrimoKeyState KeyState { get; }
        public KeyEventArgs(PrimoKeyState keyState)
        {
            KeyState = keyState;
        }
    }
}
