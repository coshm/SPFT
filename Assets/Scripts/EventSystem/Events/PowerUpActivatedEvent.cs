using SPFT.PowerUpSystem.PowerUps;

namespace SPFT.EventSystem.Events {

    public struct PowerUpActivatedEvent : IEvent {
        public IPowerUp powerUp;
    }

}
