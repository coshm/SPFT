using System;

namespace SPFT.Events
{
    public interface IEventPayload { }
    public struct EmptyPayload : IEventPayload { }

    public delegate void EventListener<E, P>(E e) where E : IEvent<P> where P : IEventPayload;

    public interface IEvent<P> where P : IEventPayload
    {
        P Payload { get; }
    }
}