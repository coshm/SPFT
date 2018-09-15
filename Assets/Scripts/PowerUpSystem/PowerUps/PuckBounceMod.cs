using System;
using UnityEngine;

namespace SPFT.PowerUpSystem.PowerUps {

    public class PuckBounceMod : PowerUpBase {

        private const int INIT_ARG_COUNT = 4;

        // Initialization Arg Names
        private const string PUCK_BOUNCE_MOD = "puckBounceMod";
        private const string PWR_UP_DURATION = "pwrUpDuration";

        // Fields that should be set during Initialization.
        #region
        public float puckBounceMod;
        public float pwrUpDuration;
        #endregion

        private Rigidbody2D puck;
        private float originalPuckBounce;

        public override void Initialize(params PowerUpArg[] args) {
            Debug.Log($"Initializing {GetType()} with args={args}");
            InitializeBase(args);
            if (args.Length != INIT_ARG_COUNT) {
                throw new InvalidOperationException($"Expected {INIT_ARG_COUNT} init args but actually got {args.Length}.");
            }
            
            foreach (PowerUpArg arg in args) {
                switch (arg.name) {
                    case ID:
                    case ICON:
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

        public override void Activate() {
            Debug.Log("Activating PuckBounceMod PowerUp.");
            IsActive = true;
            originalPuckBounce = puck.sharedMaterial.bounciness;
            puck.sharedMaterial.bounciness = puckBounceMod;
        }

        public override void Deactivate() {
            Debug.Log("Deactivating PuckBounceMod PowerUp.");
            IsActive = false;
            puck.sharedMaterial.bounciness = originalPuckBounce;
            Destroy(this);
        }

    }
}
