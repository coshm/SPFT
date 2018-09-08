namespace SPFT.EventSystem.Events
{
    public enum PowerUpActivationType
    {
        IMMEDIATE,
        MANUAL
    }

    public struct PowerUpAcquiredEvent : IEvent
    {
        public IPowerUp powerUp;
        public PowerUpActivationType activationType;
    }
}