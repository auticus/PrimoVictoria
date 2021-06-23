using System;
using System.Linq;
using UnityEngine;

namespace PrimoVictoria.Core.Events
{
    /// <summary>
    /// Handles all game events and delegates them to where they need to go (Observer Pattern)
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        private IEventQueue<EventArgs> _emptyArgsQueue;
        private IEventQueue<MouseClickEventArgs> _mouseClickQueue;
        private IEventQueue<MouseClickGamePieceEventArgs> _mouseClickGamePieceQueue;
        private IEventQueue<MovementArgs> _movementQueue;
        private IEventQueue<StandLocationArgs> _standLocationQueue;
        private IEventQueue<UserInterfaceArgs> _userInterfaceQueue;

        private static EventManager _eventManager;

        public static EventManager Instance
        {
            get
            {
                if (_eventManager == null)
                {
                    _eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                    if (_eventManager == null)
                    {
                        Debug.LogError("An event manager must be active on a GameObject in the scene");
                    }
                    else
                    {
                        _eventManager.Init();
                    }
                }

                return _eventManager;
            }
        }

        private void Init()
        {
            _emptyArgsQueue = new EventQueue<EventArgs>();
            _mouseClickQueue = new EventQueue<MouseClickEventArgs>();
            _mouseClickGamePieceQueue = new EventQueue<MouseClickGamePieceEventArgs>();
            _movementQueue = new EventQueue<MovementArgs>();
            _standLocationQueue = new EventQueue<StandLocationArgs>();
            _userInterfaceQueue = new EventQueue<UserInterfaceArgs>();
        }

        public static void Subscribe<T>(PrimoEvents e, Action<T> listener)
        {
            if (!IsValidType<T>(e))
            {
                Debug.LogError($"Exception attempting to attach Event {e} to Args Type {typeof(T)}");
                return;
            }

            var queue = GetQueue<T>();
            queue.Subscribe(e, listener);
        }

        public static void CancelSubscription<T>(PrimoEvents e, Action<T> listener)
        {
            var queue = GetQueue<T>();
            queue.CancelSubscription(e, listener);
        }

        public static void Publish<T>(PrimoEvents e, T args)
        {
            var queue = GetQueue<T>();
            queue.Publish(e, args);
        }

        private static IEventQueue<T> GetQueue<T>()
        {
            if (typeof(T) == typeof(MouseClickEventArgs)) return Instance._mouseClickQueue as IEventQueue<T>;
            if (typeof(T) == typeof(MouseClickGamePieceEventArgs)) return Instance._mouseClickGamePieceQueue as IEventQueue<T>;
            if (typeof(T) == typeof(MovementArgs)) return Instance._movementQueue as IEventQueue<T>;
            if (typeof(T) == typeof(StandLocationArgs)) return Instance._standLocationQueue as IEventQueue<T>;
            if (typeof(T) == typeof(EventArgs)) return Instance._emptyArgsQueue as IEventQueue<T>;
            if (typeof(T) == typeof(UserInterfaceArgs)) return Instance._userInterfaceQueue as IEventQueue<T>;

            throw new ArgumentException($"The type {typeof(T)} passed is not a valid queue in the EventManager");
        }

        private static bool IsValidType<T>(PrimoEvents e)
        {
            var enumType = typeof(PrimoEvents);
            var memberInfos = enumType.GetMember(e.ToString());
            var enumMemberInfo = memberInfos.FirstOrDefault(p => p.DeclaringType == enumType);
            var valueAttribute = enumMemberInfo.GetCustomAttributes(typeof(PrimoEventsAttribute), inherit: false);
            var eType = ((PrimoEventsAttribute) valueAttribute[0]).ArgsType;

            return eType == typeof(T);
        }
    }
}
