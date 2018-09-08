using System;

public class PowerUpAcquiredPayload : IEventPayload {

    public enum ActivationType {
        IMMEDIATE,
        MANUAL
    }

    public IPowerUp PowerUp { get; private set; }
    public ActivationType Type { get; private set; }

    public PowerUpAcquiredPayload(IPowerUp powerUp, ActivationType type) {
        PowerUp = powerUp;
        Type = type;
    }

    public Type GetPayloadType() {
        return GetType();
    }

}