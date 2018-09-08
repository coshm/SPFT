using System;

public class PuckResetPayload2 : IEventPayload {

    public Type GetPayloadType() {
        return GetType();
    }

}
