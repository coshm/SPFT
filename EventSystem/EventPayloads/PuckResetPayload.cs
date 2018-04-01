using System;

public class PuckResetPayload : IEventPayload {

    public Type GetPayloadType() {
        return GetType();
    }

}
