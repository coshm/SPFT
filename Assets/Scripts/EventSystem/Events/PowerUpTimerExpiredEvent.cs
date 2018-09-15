using System;

namespace SPFT.EventSystem.Events {

    public struct PowerUpTimerExpiredEvent : IEvent {
        public Guid timerId;
    }

}
