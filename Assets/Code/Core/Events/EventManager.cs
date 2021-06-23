﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrimoVictoria.Core.Events
{
    /// <summary>
    /// Handles all game events and delegates them to where they need to go (Observer Pattern)
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        private static readonly Dictionary<Type, object> queues = new Dictionary<Type, object>();

        public static void Subscribe<T>(PrimoEvents e, Action<T> listener) where T: EventArgs
        {
            if (!IsValidType<T>(e))
            {
                Debug.LogError($"Exception attempting to attach Event {e} to Args Type {typeof(T)}");
                return;
            }

            var queue = GetQueue<T>();
            queue.Subscribe(e, listener);
        }

        public static void CancelSubscription<T>(PrimoEvents e, Action<T> listener) where T: EventArgs
        {
            var queue = GetQueue<T>();
            queue.CancelSubscription(e, listener);
        }

        public static void Publish<T>(PrimoEvents e, T args) where T: EventArgs
        {
            var queue = GetQueue<T>();
            queue.Publish(e, args);
        }

        private static IEventQueue<T> GetQueue<T>() where T: EventArgs
        {
            if (queues.TryGetValue(typeof(T), out var queue))
            {
                return queue as IEventQueue<T>;
            }
            else
            {
                var newQueue = new EventQueue<T>();
                queues.Add(typeof(T), newQueue);
                return newQueue;
            }
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
