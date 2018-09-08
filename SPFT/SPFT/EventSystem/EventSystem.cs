using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;

namespace SPFT.EventSystem
{
    /* Base Event interfaces and classes */

    public interface IEvent { }
    public delegate void EventListener<E>(E e) where E : IEvent;

    /* EventManagement that handles registering listeners and notifying them when an event happens */

    public class EventManager : MonoBehaviour
    {
        private static EventManager eventMgr;
        public static EventManager Instance
        {
            get
            {
                if (!eventMgr)
                {
                    eventMgr = FindObjectOfType(typeof(EventManager)) as EventManager;
                    if (!eventMgr)
                    {
                        Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                    }
                    else
                    {
                        eventMgr.Init();
                    }
                }
                return eventMgr;
            }
        }

        void Init() { }

        private Dictionary<Type, List<Delegate>> listenersByEventType;

        void Awake()
        {
            listenersByEventType = new Dictionary<Type, List<Delegate>>();
        }

        public void RegisterListener<E>(EventListener<E> listener) where E : IEvent
        {
            Type eventType = typeof(E);
            if (!listenersByEventType.ContainsKey(eventType))
            {
                listenersByEventType.Add(eventType, new List<Delegate>());
            }

            listenersByEventType[eventType].Add(listener);
        }

        public void NotifyListeners<E>(E spftEvent) where E : IEvent
        {
            Type eventType = typeof(E);
            if (!listenersByEventType.ContainsKey(eventType))
            {
                return;
            }

            List<Delegate> listeners = listenersByEventType[eventType];
            foreach (Delegate genericDelegate in listeners)
            {
                EventListener<E> listener = (EventListener<E>)genericDelegate;
                listener(spftEvent);
            }
        }

    }
}
