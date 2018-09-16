using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace SPFT.PowerUpSystem.PowerUps {

    public class Teleporter : PowerUpBase {

        private const int INIT_ARG_COUNT = 7;

        // Initialization Arg Names
        private const string MINIMUM_Y_TO_TELEPORT = "minimumYToTeleport";
        private const string TELEPORT_EXIT_X_BOUNDS = "teleportExitXBounds";
        private const string TELEPORT_EXIT_Y_BOUNDS = "teleportExitYBounds";
        private const string TELEPORT_ENTER_CLIP = "teleportEnterClip";
        private const string TELEPORT_EXIT_CLIP = "teleportExitClip";

        // Fields that should be set during Initialization.
        public float minimumYToTeleport;    // The lowest y value the player can activate teleport at
        public Vector2 teleportExitXBounds; // The range of x values the player can be teleported to
        public Vector2 teleportExitYBounds; // The range of y values the player can be teleported to
        public AnimationClip teleportEnterClip;
        public AnimationClip teleportExitClip;

        private Rigidbody2D puck;
        private float puckColliderRadius;
        private Vector2 puckVelocity;
        private float puckAngularVel;

        public override void Initialize(params PowerUpArg[] args) {
            InitializeBase(args);
            if (args.Length != INIT_ARG_COUNT) {
                throw new InvalidOperationException($"Expected {INIT_ARG_COUNT} init args but actually got {args.Length}.");
            }

            foreach (PowerUpArg arg in args) {
                switch (arg.name) {
                    case MINIMUM_Y_TO_TELEPORT:
                        minimumYToTeleport = Utilities.ConvertStringOrDefault(arg.value, gameSettings.minimumYToTeleport);
                        break;
                    case TELEPORT_EXIT_X_BOUNDS:
                        teleportExitXBounds = Utilities.ConvertStringOrDefault(arg.value, gameSettings.teleportExitXBounds);
                        break;
                    case TELEPORT_EXIT_Y_BOUNDS:
                        teleportExitYBounds = Utilities.ConvertStringOrDefault(arg.value, gameSettings.teleportExitYBounds);
                        break;
                    case TELEPORT_ENTER_CLIP:
                        teleportEnterClip = ResourceLoader.LoadResource<AnimationClip>(arg.value);
                        break;
                    case TELEPORT_EXIT_CLIP:
                        teleportExitClip = ResourceLoader.LoadResource<AnimationClip>(arg.value);
                        break;
                    default:
                        Debug.LogWarning($"No parameter found for {arg.name}");
                        break;
                }
            }
        }

        void Start() {
            puck = PowerUpLifeCycleManager.Instance.puck.GetComponent<Rigidbody2D>();
            puckColliderRadius = puck.GetComponent<CircleCollider2D>().radius;
        }

        public override void Activate() {
            Debug.Log($"Activating {GetType()} PowerUp.");
            IsActive = true;

            // Cache vel and angular vel before turning off physics
            Vector2 velocity = puck.velocity;
            float angularVelocity = puck.angularVelocity;

            // Turn off physics for the Puck
            puck.velocity = Vector2.zero;
            puck.angularVelocity = 0f;
            puck.bodyType = RigidbodyType2D.Kinematic;

            // Find a safe place to teleport to
            Vector2 teleportTo = FindTeleportExit();

            // Play teleport animations
            //StartCoroutine(PlayTeleportAnimations());

            // Move Puck to teleport position and re-enable physics
            puck.position = teleportTo;
            puck.velocity = velocity;
            puck.angularVelocity = angularVelocity;
            puck.bodyType = RigidbodyType2D.Dynamic;
            puck.WakeUp();

            // Teleportation is done, Deactivate
            Deactivate();
        }

        public override void Deactivate() {
            Debug.Log($"Deactivating {GetType()} PowerUp.");

            IsActive = false;

            EmitExpiredEventAndSelfDestruct(this, gameSettings.pwrUpPostDeactivationDelay);
        }

        // Block activation if the Puck is below the Activation Y Bound
        public override bool IsBlocked(List<IPowerUp> activePowerUps) {
            if (puck.position.y < minimumYToTeleport) {
                return true;
            }

            foreach (IPowerUp powerUp in activePowerUps) {
                if (powerUp.GetType() == this.GetType()) {
                    return true;
                }
            }

            return false;
        }

        /* ~~~~~~~~~~~~~~~~~~~~ Teleporter Specific Methods ~~~~~~~~~~~~~~~~~~~~ */

        // Pick a random point within the given teleport bounds until we 
        //  find one that's safe to teleport to.
        private Vector2 FindTeleportExit() {
            float x, y;
            Vector2 teleportTo;
            do {
                x = Random.Range(teleportExitXBounds.x, teleportExitXBounds.y);
                y = Random.Range(teleportExitYBounds.x, teleportExitYBounds.y);
                teleportTo = new Vector2(x, y);
                Debug.Log($"Checking if it's safe to teleport to {teleportTo}");
            } while (IsPegBlockingTeleportation(teleportTo));
            return teleportTo;
        }

        // Plays the enter and exit Teleport animations, returns when they are both done.
        private IEnumerator PlayTeleportAnimations(Vector2 teleportTo) {
            // Play teleportEnterClip at puck.position

            // Play teleportExitClip at teleportTo

            // Figure out how to return only when those animations are done
            yield return new WaitForSeconds(0.5f);
        }

        private bool IsPegBlockingTeleportation(Vector2 newPos) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(newPos, puckColliderRadius + 0.1f);
            if (colliders == null || colliders.Length == 0) {
                return false;
            }

            foreach (Collider2D collider in colliders) {
                if (collider.CompareTag(gameSettings.pegTag)) {
                    return true;
                }
            }
            return false;
        }
    }
}