using System;

namespace PrimoVictoria.Core.Events
{
    public interface IEventQueue<T>
    {
        void Subscribe(PrimoEvents e, Action<T> listener);
        void CancelSubscription(PrimoEvents e, Action<T> listener);
        void Publish(PrimoEvents e, T args);
    }
}
