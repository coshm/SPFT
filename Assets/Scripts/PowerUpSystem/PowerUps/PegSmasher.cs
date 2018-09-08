using UnityEngine;
using System.Collections;
using System;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;
using System.Collections.Generic;


namespace SPFT.PowerUpSystem.PowerUps {

    public class PegSmasher : MonoBehaviour, IPowerUp {

        private const int INIT_ARG_COUNT = 5;

        // Initialization Arg Names
        #region
        private const string ID = "id";
        private const string PUCK_VELOCITY_MOD = "puckVelocityMod";
        private const string MAX_PEG_BREAKS = "maxPegBreaks";
        private const string STUTTER_LENGTH = "stutterDuration";
        private const string PEG_RESPAWN_DELAY = "pegRespawnDelay";
        #endregion

        // Fields that should be set during Initialization.
        #region
        public Guid Id { get; private set; }
        public bool IsActive { get; private set; }
        public float puckVelocityMod;
        public int maxPegBreaks;
        public float stutterDuration;
        public float pegRespawnDelay;
        #endregion

        private GameSettings gameSettings;

        private Vector2 originalPuckVel;
        private int pegBreakCount;

        public void Initialize(params PowerUpArg[] args) {
            if (args.Length != INIT_ARG_COUNT) {
                throw new InvalidOperationException($"Expected {INIT_ARG_COUNT} init args but actually got {args.Length}.");
            }

            foreach (PowerUpArg arg in args) {
                switch (arg.name) {
                    case ID:
                        Id = Guid.Parse(arg.value);
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
                    default:
                        throw new InvalidOperationException($"No parameter found for {arg.name}");
                }
            }
        }

        void Awake() {
            gameSettings = GameSettings.Instance;
            IsActive = false;
        }

        void Start() {
            EventManager.Instance.RegisterListener<PuckPegCollisionEvent>(OnPuckPegCollision);
        }

        public void Activate() {
            Debug.Log("Activating PegSmasher PowerUp.");

            IsActive = true;

            // Boost the players velocity at the time of Activation
            Rigidbody2D puck = PowerUpManager.Instance.puck.GetComponent<Rigidbody2D>();
            puck.velocity *= puckVelocityMod;
            pegBreakCount = 0;

            StartCoroutine(StutterPuckMovement(puck));
        }

        public void Deactivate() {
            Debug.Log("Deactivating PegSmasher PowerUp.");
            IsActive = false;
            Destroy(this);
        }

        public bool IsBlocked(List<IPowerUp> activePwrUps) {
            return false;
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
            peg.Explode(impactDir, pegRespawnDelay);
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

        //private void DampenPuckMomentum(Peg peg, Puck puck) {
        //    Debug.Log($"Attempting to Dampen Puck Momentum after {pegBreakCount} broken Pegs.");

        //    Rigidbody2D puckBody = puck.GetComponent<Rigidbody2D>();

        //    Vector2 n = puck.transform.position - peg.transform.position;
        //    n.Normalize();
        //    Debug.Log($"Normal Vectory={n}");

        //    Vector2 puckVelocity = puckBody.velocity;
        //    Debug.Log($"Current Puck Velocity={puckVelocity}");

        //    float p = puckVelocity.x * n.x + puckVelocity.y * n.y;
        //    Debug.Log($"Whatever tf P is={p}");

        //    Vector2 newPuckVelocity = puckVelocity - 2 * p * n;
        //    Debug.Log($"New Puck Velocity={newPuckVelocity}");

        //    puckBody.velocity = newPuckVelocity;
        //}

        //private void DampenPuckMomentum(Peg peg, Puck puck) {
        //    Debug.Log($"Attempting to Dampen Puck Momentum after {pegBreakCount} broken Pegs.");

        //    Vector2 puckForce = puck.GetPuckAcceleration();
        //    Vector2 pegForce = puck.transform.position - peg.transform.position;

        //    Debug.Log($"PuckForce={puckForce}, PegForce={pegForce}.");

        //    // Calculate what the speed of the puck should be based on how many 
        //    // pegs have been broken out of the max number that can be broken.
        //    double pegBreakCountMod = Math.Pow((double) pegBreakCount / maxPegBreaks, 2.0);
        //    Debug.Log($"PegBreakCountMod={pegBreakCountMod}.");

        //    float puckDotPeg = Vector2.Dot(puckForce, pegForce);
        //    Debug.Log($"PuckDotPeg={puckDotPeg}.");

        //    Vector2 dampeningForce = (float) (pegBreakCountMod * Math.Abs(puckDotPeg)) * pegForce;
        //    Debug.Log($"DampeningForce={dampeningForce}.");

        //    // Apply Dampening force to Puck
        //    puck.GetComponent<Rigidbody2D>().AddForce(dampeningForce, ForceMode2D.Impulse);
        //}

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
