using System;

public class PowerUpExpiredPayload2 : IEventPayload {

    public IPowerUp PowerUp { get; private set; }

    public PowerUpExpiredPayload(IPowerUp powerUp) {
        PowerUp = powerUp;
    }

    public Type GetPayloadType() {
        return GetType();
    }
}
