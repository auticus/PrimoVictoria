using System;
using System.Collections.Generic;

namespace PrimoVictoria.Core.Events
{
    /// <summary>
    /// Primo Event Queue used to handle messages and pass them to subscribers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventQueue<T> : IEventQueue<T> where T: PrimoBaseEventArgs
    {
        private readonly Dictionary<PrimoEvents, Action<T>> _eventDictionary;

        public EventQueue()
        {
            _eventDictionary = new Dictionary<PrimoEvents, Action<T>>();
        }

        public void Subscribe(PrimoEvents e, Action<T> listener)
        {
            if (_eventDictionary.TryGetValue(e, out var thisEvent))
            {
                //event already exists so just add this listener 
                thisEvent += listener;
                _eventDictionary[e] = thisEvent;
            }
            else
            {
                //event was not yet listed so add it to the dictionary
                thisEvent += listener;
                _eventDictionary.Add(e, thisEvent);
            }
        }

        public void CancelSubscription(PrimoEvents e, Action<T> listener)
        {
            if (_eventDictionary.TryGetValue(e, out var thisEvent))
            {
                thisEvent -= listener;
                _eventDictionary[e] = thisEvent;
            }
        }

        public void Publish(PrimoEvents e, T args)
        {
            if (_eventDictionary.TryGetValue(e, out var thisEvent))
            {
                thisEvent.Invoke(args);
            }
        }
    }
}
