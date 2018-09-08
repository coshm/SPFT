using System;
using System.Collections.Generic;
using UnityEngine;
using SPFT.EventSystem.Events;

namespace SPFT.EventSystem {

    /* Base Event Interface and EventListener Delegate. All Events will use these. */
    public delegate void EventListener<E>(E e) where E : IEvent;

    /* This is the actual EventManager that handles registering listeners and notifying them of relevant Events */
    public class EventManager : MonoBehaviour {

        private static EventManager eventMgr;
        public static EventManager Instance
        {
            get
            {
                if (!eventMgr) {
                    eventMgr = FindObjectOfType(typeof(EventManager)) as EventManager;
                    if (!eventMgr) {
                        Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                    } else {
                        eventMgr.Init();
                    }
                }
                return eventMgr;
            }
        }

        void Init() { }

        private Dictionary<Type, List<Delegate>> listenersByEventType;

        void Awake() {
            listenersByEventType = new Dictionary<Type, List<Delegate>>();
        }

        public void RegisterListener<E>(EventListener<E> listener) where E : IEvent {
            Type eventType = typeof(E);
            if (!listenersByEventType.ContainsKey(eventType)) {
                listenersByEventType.Add(eventType, new List<Delegate>());
            }

            listenersByEventType[eventType].Add(listener);
        }

        public void NotifyListeners<E>(E spftEvent) where E : IEvent {
            Type eventType = typeof(E);
            Debug.Log($"Notifying listeners of {eventType}");

            if (!listenersByEventType.ContainsKey(eventType)) {
                return;
            }

            List<Delegate> genericDelegates = listenersByEventType[eventType];
            foreach (Delegate genericDelegate in genericDelegates) {
                EventListener<E> listener = (EventListener<E>) genericDelegate;
                listener(spftEvent);
            }
        }
    }
}