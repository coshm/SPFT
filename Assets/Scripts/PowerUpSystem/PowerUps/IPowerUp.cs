using System;
using System.Collections.Generic;
using UnityEngine;

namespace SPFT.PowerUpSystem.PowerUps {

    public interface IPowerUp {

        void Initialize(params PowerUpArg[] args);

        Guid Id { get; }

        Sprite Icon { get; }

        bool IsActive { get; }

        void Activate();

        void Deactivate();

        bool IsBlocked(List<IPowerUp> activePowerUps);

    }

}