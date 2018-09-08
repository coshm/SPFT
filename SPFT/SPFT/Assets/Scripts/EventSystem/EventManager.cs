using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private static EventManager eventMgr;
    public static EventManager Instance {
        get {
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

    private List<UnityEvent> events;
    private List<UnityEvent<IEventPayload>> eventsWithPayload;

    private List<UnityAction> orphanedListeners;
    private List<UnityAction<IEventPayload>> orphanedListenersWithPayload;

    void Init() {
    }

    void Awake() {
        events = new List<UnityEvent>();
        eventsWithPayload = new List<UnityEvent<IEventPayload>>();
    }

    public UnityEvent GetOrAddEvent<E>(E unityEvent) where E : UnityEvent {
        List<E> eventsOfType = events.OfType<E>().ToList();
        if (eventsOfType.Count > 1) {
            throw new InvalidOperationException("There can only be one event for a given type. EventType=" + unityEvent.GetType());
        } else if (eventsOfType.Count > 1) {
            return eventsOfType.First();
        } else {
            events.Add(unityEvent);
            return unityEvent;
        }
    }

    public E GetOrAddEventWithPayload<E>(E unityEvent) where E : UnityEvent<IEventPayload> {
        List<E> eventsOfType = eventsWithPayload.OfType<E>().ToList();
        if (eventsOfType.Count > 1) {
            throw new InvalidOperationException("There can only be one event for a given type. EventType=" + unityEvent.GetType());
        } else if (eventsOfType.Count > 1) {
            return eventsOfType.First();
        } else {
            eventsWithPayload.Add(unityEvent);
            return unityEvent;
        }
    }

    public void RegisterListener<E>(UnityAction action) where E : UnityEvent {
        List<E> eventsOfType = events.OfType<E>().ToList();
        if (eventsOfType.Count == 0) {
            orphanedListeners.Add(action);
        } else {
            E unityEvent = eventsOfType.First();
            unityEvent.AddListener(action);
        }
    }

    public void RegisterListenerWithPayload<E>(UnityAction<IEventPayload> action) where E : UnityEvent<IEventPayload> {
        List<E> eventsOfType = eventsWithPayload.OfType<E>().ToList();
        if (eventsOfType.Count == 0) {
            orphanedListenersWithPayload.Add(action);
        } else {
            E unityEvent = eventsOfType.First();
            unityEvent.AddListener(action);
        }
    }

}
