public interface IEventListener<E> where E : IEventPayload {
    void Notify(E payload);
}

public interface IEventListener {
    void Notify();
}
