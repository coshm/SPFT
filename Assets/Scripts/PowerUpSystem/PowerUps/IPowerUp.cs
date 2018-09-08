using System;
using System.Collections.Generic;

namespace SPFT.PowerUpSystem.PowerUps {

    public interface IPowerUp {

        void Initialize(params PowerUpArg[] args);

        Guid Id { get; }

        bool IsActive { get; }

        void Activate();

        void Deactivate();

        bool IsBlocked(List<IPowerUp> activePowerUps);

    }

}