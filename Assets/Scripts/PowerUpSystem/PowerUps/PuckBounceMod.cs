using System;
using System.Collections.Generic;
using UnityEngine;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;


namespace SPFT.PowerUpSystem.PowerUps {

    public class PuckBounceMod : MonoBehaviour, IPowerUp {

        private const int INIT_ARG_COUNT = 3;

        // Initialization Arg Names
        private const string ID = "id";
        private const string PUCK_BOUNCE_MOD = "puckBounceMod";
        private const string PWR_UP_DURATION = "pwrUpDuration";

        // Fields that should be set during Initialization.
        #region
        public Guid Id { get; private set; }
        public bool IsActive { get; private set; }
        public float puckBounceMod;
        public float pwrUpDuration;
        #endregion

        private Rigidbody2D puck;
        private GameSettings gameSettings;

        private float originalPuckBounce;

        public void Initialize(params PowerUpArg[] args) {
            if (args.Length != INIT_ARG_COUNT) {
                throw new InvalidOperationException($"Expected {INIT_ARG_COUNT} init args but actually got {args.Length}.");
            }

            IsActive = false;
            foreach (PowerUpArg arg in args) {
                switch (arg.name) {
                    case ID:
                        Id = Guid.Parse(arg.value);
                        break;
                    case PUCK_BOUNCE_MOD:
                        puckBounceMod = Utilities.ConvertStringOrDefault(arg.value, gameSettings.puckBounceMod);
                        break;
                    case PWR_UP_DURATION:
                        pwrUpDuration = Utilities.ConvertStringOrDefault(arg.value, gameSettings.powerUpDuration);
                        break;
                    default:
                        throw new InvalidOperationException($"No parameter found for {arg.name}");
                }
            }
        }

        void Awake() {
            puck = PowerUpManager.Instance.puck.GetComponent<Rigidbody2D>();
            gameSettings = GameSettings.Instance;
        }

        void Update() {
            if (IsActive) {
                pwrUpDuration -= Time.deltaTime;
                if (pwrUpDuration <= 0f) {
                    pwrUpDuration = 0f;
                    Deactivate();
                }
                PowerUpManager.Instance.UpdateTimerDisplay(pwrUpDuration);
            }
        }

        public void Activate() {
            Debug.Log("Activating PuckBounceMod PowerUp.");
            IsActive = true;
            originalPuckBounce = puck.sharedMaterial.bounciness;
            puck.sharedMaterial.bounciness = puckBounceMod;
        }

        public void Deactivate() {
            Debug.Log("Deactivating PuckBounceMod PowerUp.");
            IsActive = false;
            puck.sharedMaterial.bounciness = originalPuckBounce;
            Destroy(this);
        }

        public bool IsBlocked(List<IPowerUp> activePwrUps) {
            return false;
        }
    }
}
