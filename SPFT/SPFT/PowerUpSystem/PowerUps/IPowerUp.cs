using System;

public interface IPowerUp {

    void Initialize(params string[] args);

    Guid Id { get; }

    Sprite Icon { get; }

    bool IsActive { get; }

    void Activate();

    void Deactivate();

    bool IsBlockingPowerUpActivation(IPowerUp pwrUp);

    bool OnPowerUpTrigger(IPowerUpTrigger pwrUpTrigger);

}