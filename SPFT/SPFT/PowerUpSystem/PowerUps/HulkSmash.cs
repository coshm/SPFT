using UnityEngine;
using System.Collections;
using System;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;

public class HulkSmash : MonoBehaviour, IPowerUp {

    private const float DEFAULT_PUCK_VEL_MOD = 2f;
    private const int DEFAULT_MAX_PEG_BREAKS = 2;
    private const float DEFAULT_STUTTER_LENGTH = 0.1f;

    public Guid Id { get; private set; }
    public bool IsActive { get; private set; }

    // Public so they can be set in editor
    public float puckVelModifier;
    public int maxPegBreaks;

    private Vector2 originalPuckVel;
    private int pegBreakCount;
    private Rigidbody2D puck;

    void Awake() {
        Id = Guid.NewGuid();

        if (puckVelModifier == 0f) {
            puckVelModifier = DEFAULT_PUCK_VEL_MOD;
        }
        if (maxPegBreaks == 0) {
            maxPegBreaks = DEFAULT_MAX_PEG_BREAKS;
        }
    }

    void Start() {

    }

    void Update() {

    }

    public Guid GetId() {
        return Id;
    }

    public void Activate() {
        pegBreakCount = 0;
        puck = PowerUpManager.Instance.puck.GetComponent<Rigidbody2D>();
        originalPuckVel = puck.velocity;

        StartCoroutine("StutterPuckMovement");

        puck.velocity = originalPuckVel * puckVelModifier;
        IsActive = true;
    }

    public void Deactivate() {
        if (IsActive) {
            PowerUpExpiredEvent pwrUpExpiredEvent = new PowerUpExpiredEvent()
            {
                powerUp = this,
            };
            EventManager.Instance.NotifyListeners<PowerUpExpiredEvent>(pwrUpExpiredEvent);

            IsActive = false;
        }
    }

    public bool IsBlockingPowerUpActivation(IPowerUp pwrUp) {
        return false;
    }
    
    public bool OnPowerUpTrigger(IPowerUpTrigger pwrUpTrigger) {
        if (pwrUpTrigger.GetType() == typeof(PuckCollisionTrigger)) {
            PuckCollisionTrigger puckCollTrigger = (PuckCollisionTrigger)pwrUpTrigger;
            Collision2D coll = puckCollTrigger.Coll;

            if (coll.gameObject.tag == "Peg" && pegBreakCount < maxPegBreaks) {
                Puck puck = puckCollTrigger.Puck;

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

    private void UpdatePuckVelPerPegCount() {
        // Calculate what the speed of the puck should be based on how many 
        // pegs have been broken out of the max number that can be broken.
        float pegBreakPercentage = (float)pegBreakCount / maxPegBreaks;
        float updatedSpeed = originalPuckVel.magnitude * (puckVelModifier * (1 - pegBreakPercentage) + pegBreakPercentage);

        // Update the speed but don't change the direction.
        Vector2 velocityDir = puck.velocity / puck.velocity.magnitude;
        puck.velocity = velocityDir * updatedSpeed;
    }

    private IEnumerator StutterPuckMovement() {
        // Save vel and angular vel before turning off physics
        Vector2 velocity = puck.velocity;
        float angularVelocity = puck.angularVelocity;
        puck.bodyType = RigidbodyType2D.Kinematic;

        yield return new WaitForSeconds(DEFAULT_STUTTER_LENGTH);

        // Restore vel and angular vel before reenabling physics
        puck.velocity = velocity;
        puck.angularVelocity = angularVelocity;
        puck.bodyType = RigidbodyType2D.Dynamic;
        puck.WakeUp();
    }


}
