using SPFT.PowerUpSystem.PowerUps;

namespace SPFT.EventSystem.Events {
    
    public struct SlotMachineCompletedEvent : IEvent {
        public IPowerUp powerUp;
    }

}
