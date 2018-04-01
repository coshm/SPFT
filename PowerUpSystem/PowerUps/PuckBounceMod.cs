using System;
using UnityEngine;

public class PuckBounceMod : MonoBehaviour, IPowerUp {

    private const float DEFAULT_PUCK_BOUNCE_MOD = 2f;
    private const float DEFAULT_PWR_UP_DURATION = 30f;

    public Guid Id { get; private set; }
    public bool IsActive { get; private set; }

    // Public so they can be set in editor
    public float puckBounceMod;
    public float pwrUpDuration;

    private float originalPuckBounce;
    private Rigidbody2D puck;
    private bool isActive;

    public PowerUpAcquiredEvent pwrUpAcquiredEvent;
    public PowerUpExpiredEvent pwrUpExpiredEvent;

    void Awake() {
        Id = Guid.NewGuid();

        pwrUpAcquiredEvent = EventManager.Instance.GetOrAddEventWithPayload(new PowerUpAcquiredEvent());
        pwrUpExpiredEvent = EventManager.Instance.GetOrAddEventWithPayload(new PowerUpExpiredEvent());

        if (puckBounceMod == 0f) {
            puckBounceMod = DEFAULT_PUCK_BOUNCE_MOD;
        }
        if (pwrUpDuration == 0f) {
            pwrUpDuration = DEFAULT_PWR_UP_DURATION;
        }
    }

    void Start() {

    }

    void Update() {
        if (IsActive) {
            pwrUpDuration -= Time.deltaTime;
            if (pwrUpDuration <= 0f) {
                pwrUpDuration = 0f;
                Deactivate();
            }
            PowerUpManager.Instance.UpdateTimerText(pwrUpDuration);
        }
    }

    public void Activate() {
        puck = PowerUpManager.Instance.puck.GetComponent<Rigidbody2D>();
        originalPuckBounce = puck.sharedMaterial.bounciness;
        puck.sharedMaterial.bounciness = puckBounceMod;
        IsActive = true;
    }

    public void Deactivate() {
        if (isActive) {
            puck.sharedMaterial.bounciness = originalPuckBounce;

            PowerUpExpiredPayload pwrUpExpiredPayload = new PowerUpExpiredPayload(this);
            pwrUpExpiredEvent.Invoke(pwrUpExpiredPayload);

            IsActive = false;
        }
    }

    public bool IsBlockingPowerUpActivation(IPowerUp pwrUp) {
        return false;
    }
    
    public bool OnPowerUpTrigger(IPowerUpTrigger pwrUpTrigger) {
        return false;
    }
}
