using UnityEngine;
using System.Collections;
using System;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;


namespace SPFT.PowerUpSystem.PowerUps {

    public class PegSmasher : PowerUpBase {

        private const int INIT_ARG_COUNT = 6;

        // Initialization Arg Names
        #region
        private const string PUCK_VELOCITY_MOD = "puckVelocityMod";
        private const string MAX_PEG_BREAKS = "maxPegBreaks";
        private const string STUTTER_LENGTH = "stutterDuration";
        private const string PEG_RESPAWN_DELAY = "pegRespawnDelay";
        private const string PEG_BREAK_CLIP = "pegBreakClip";
        #endregion

        // Fields that should be set during Initialization.
        #region
        public float puckVelocityMod;
        public int maxPegBreaks;
        public float stutterDuration;
        public float pegRespawnDelay;
        public AnimationClip pegBreakClip;
        #endregion

        private Vector2 originalPuckVel;
        private int pegBreakCount;

        public override void Initialize(params PowerUpArg[] args) {
            Debug.Log($"Initializing {GetType()} with args={args}");

            if (args.Length != INIT_ARG_COUNT) {
                throw new InvalidOperationException($"Expected {INIT_ARG_COUNT} init args but actually got {args.Length}.");
            }
            
            foreach (PowerUpArg arg in args) {
                switch (arg.name) {
                    case ID:
                    case ICON:
                        InitializeBase(arg);
                        break;
                    case PUCK_VELOCITY_MOD:
                        puckVelocityMod = Utilities.ConvertStringOrDefault(arg.value, gameSettings.puckVelocityMod);
                        break;
                    case MAX_PEG_BREAKS:
                        maxPegBreaks = Utilities.ConvertStringOrDefault(arg.value, gameSettings.maxPegBreaks);
                        break;
                    case STUTTER_LENGTH:
                        stutterDuration = Utilities.ConvertStringOrDefault(arg.value, gameSettings.stutterDuration);
                        break;
                    case PEG_RESPAWN_DELAY:
                        pegRespawnDelay = Utilities.ConvertStringOrDefault(arg.value, gameSettings.pegRespawnDelay);
                        break;
                    case PEG_BREAK_CLIP:
                        pegBreakClip = Resources.Load<AnimationClip>(arg.value); // TODO: figure out how to load/play animation clips
                        break;
                    default:
                        throw new InvalidOperationException($"No parameter found for {arg.name}");
                }
            }
        }
        
        void Start() {
            EventManager.Instance.RegisterListener<PuckPegCollisionEvent>(OnPuckPegCollision);
        }

        public override void Activate() {
            Debug.Log("Activating PegSmasher PowerUp.");

            IsActive = true;

            // Boost the players velocity at the time of Activation
            Rigidbody2D puck = PowerUpManager.Instance.puck.GetComponent<Rigidbody2D>();
            puck.velocity *= puckVelocityMod;
            pegBreakCount = 0;

            StartCoroutine(StutterPuckMovement(puck));
        }

        public override void Deactivate() {
            Debug.Log("Deactivating PegSmasher PowerUp.");
            IsActive = false;
            Destroy(this);
        }

        public void OnPuckPegCollision(PuckPegCollisionEvent puckPegCollisionEvent) {
            if (IsActive && pegBreakCount < maxPegBreaks) {
                Puck puck = puckPegCollisionEvent.puck;
                Rigidbody2D puckBody = puck.GetComponent<Rigidbody2D>();
                Collider2D pegCollider = puckPegCollisionEvent.collision.collider;
                Peg peg = pegCollider.GetComponent<Peg>();

                // Ignore collision with this peg
                Collider2D puckCollider = puck.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(pegCollider, puckCollider);

                // Smash Peg
                SmashPeg(pegCollider, puckBody);
                pegBreakCount++;

                // Dampen momentum of Puck after it smashes the Peg
                DampenPuckMomentum(peg, puck);

                StartCoroutine(StutterPuckMovement(puckBody));

                if (pegBreakCount >= maxPegBreaks) {
                    Deactivate();
                }
            }
        }

        private void SmashPeg(Collider2D pegCollider, Rigidbody2D puckRigidBody) {
            Vector2 puckVelocity = puckRigidBody.velocity;
            puckVelocity.Normalize();

            Vector2 impactVector = (Vector2) pegCollider.transform.position - puckVelocity;
            Vector2 impactDir = impactVector / impactVector.magnitude;

            Peg peg = pegCollider.gameObject.GetComponent<Peg>();
            peg.Destroy(pegRespawnDelay);

            // TODO: Determine how to use pegBreakClip here
        }

        private void DampenPuckMomentum(Peg peg, Puck puck) {
            Debug.Log($"Attempting to Dampen Puck Momentum after {pegBreakCount} broken Pegs.");

            Rigidbody2D puckBody = puck.GetComponent<Rigidbody2D>();

            Vector2 collisionNormal = puck.transform.position - peg.transform.position;
            collisionNormal.Normalize();
            Debug.Log($"Normal Vector={collisionNormal}");

            Vector2 puckVelocity = puckBody.velocity;
            Debug.Log($"Current Puck Velocity={puckVelocity}");

            Vector2 projVOnN = puckVelocity * (float) (Math.Abs(Vector2.Dot(puckVelocity, collisionNormal)) / Math.Pow(puckVelocity.magnitude, 2));
            Debug.Log($"Projection of Vel onto Normal={projVOnN}");

            float pegBreakCountMod = (float) Math.Pow((double) pegBreakCount / maxPegBreaks, 2.0);
            Debug.Log($"pegBreakCountMod={pegBreakCountMod}");

            Vector2 newPuckVelocity = puckVelocity + pegBreakCountMod * projVOnN;
            Debug.Log($"New Puck Velocity={newPuckVelocity}");

            puckBody.velocity = newPuckVelocity;
        }

        private IEnumerator StutterPuckMovement(Rigidbody2D puck) {
            // Save vel and angular vel before turning off physics
            Vector2 velocity = puck.velocity;
            float angularVelocity = puck.angularVelocity;

            // Turn off physics for the Puck
            puck.velocity = Vector2.zero;
            puck.angularVelocity = 0f;
            puck.bodyType = RigidbodyType2D.Kinematic;

            yield return new WaitForSeconds(stutterDuration);

            // Restore vel and angular vel before reenabling physics
            puck.velocity = velocity;
            puck.angularVelocity = angularVelocity;
            puck.bodyType = RigidbodyType2D.Dynamic;
            puck.WakeUp();
        }

    }
}
