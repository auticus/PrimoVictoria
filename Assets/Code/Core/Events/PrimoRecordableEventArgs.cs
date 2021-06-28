using PrimoVictoria.Core.Events;

namespace PrimoVictoria.Assets.Code.Core.Events
{
    /// <summary>
    /// Represents an event arg that will be recorded in the input buffer for playback
    /// </summary>
    public abstract class PrimoRecordableEventArgs : PrimoBaseEventArgs
    {
        protected PrimoRecordableEventArgs() : base(record: true)
        {

        }
    }
}
