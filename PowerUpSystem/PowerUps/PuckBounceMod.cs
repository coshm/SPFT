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
                PowerUpExpiredPayload pwrUpExpiredPayload = new PowerUpExpiredPayload(this);
                pwrUpExpiredEvent.Invoke(pwrUpExpiredPayload);
                Destroy(this);
            }
        }
    }

    public Guid GetId() {
        return Id;
    }

    public void Activate() {
        puck = PowerUpManager.Instance.puck.GetComponent<Rigidbody2D>();
        originalPuckBounce = puck.sharedMaterial.bounciness;
        puck.sharedMaterial.bounciness = puckBounceMod;
        IsActive = true;
    }

    public void Deactivate() {

        puck = PowerUpManager.Instance.puck.GetComponent<Rigidbody2D>();
        originalPuckBounce = puck.sharedMaterial.bounciness;
        puck.sharedMaterial.bounciness = puckBounceMod;
        IsActive = true;
    }

    public bool IsBlockingPowerUpActivation(IPowerUp pwrUp) {
        return false;
    }
    
    public bool OnPowerUpTrigger(IPowerUpTrigger pwrUpTrigger) {
        if (pwrUpTrigger.GetType() == typeof(PuckCollisionTrigger)) {
            PuckCollisionTrigger puckCollTrigger = (PuckCollisionTrigger) pwrUpTrigger;
            Collision2D coll = puckCollTrigger.Coll;
            Puck puck = puckCollTrigger.Puck;

            if (coll.gameObject.tag == "Peg" && pegBreakCount < maxPegBreaks) {
                // Ignore collision with this peg
                Collider2D puckCollider = puck.gameObject.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(coll.collider, puckCollider);

                pegBreakCount++;
                //Peg peg = coll.gameObject.GetComponent<Peg>();
                //peg.Explode();

                UpdatePuckVelPerPegCount();

                StartCoroutine("StutterPuckMovement");

                if (pegBreakCount == maxPegBreaks) {
                    PowerUpExpiredPayload pwrUpExpiredPayload = new PowerUpExpiredPayload(this);
                    pwrUpExpiredEvent.Invoke(pwrUpExpiredPayload);
                    Destroy(this);
                }
            }
            return true;
        }
        return false;
    }
}
