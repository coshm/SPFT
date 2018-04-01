using System;

public interface IPowerUp {

    Guid Id { get; }

    bool IsActive { get; }

    void Activate();

    void Deactivate();

    bool IsBlockingPowerUpActivation(IPowerUp pwrUp);

    bool OnPowerUpTrigger(IPowerUpTrigger pwrUpTrigger);

}