using System;

namespace PrimoVictoria.Core.Events
{
    public class PrimoBaseEventArgs : EventArgs
    {
        public DateTimeOffset EventTime { get; }
        public bool RecordCommand { get; }

        public PrimoBaseEventArgs()
        {
            EventTime = DateTimeOffset.UtcNow;
        }

        public PrimoBaseEventArgs(bool record) : this()
        {
            RecordCommand = record;
        }

        public new static PrimoBaseEventArgs Empty()
        {
            return new PrimoBaseEventArgs(record: false);
        }
    }
}
