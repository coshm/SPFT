﻿using SPFT.PowerUpSystem.PowerUps;

namespace SPFT.EventSystem.Events {

    public struct PowerUpExpiredEvent : IEvent {
        public IPowerUp powerUp;
    }

}
