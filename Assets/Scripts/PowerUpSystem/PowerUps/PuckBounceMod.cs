using System;
using UnityEngine;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;

namespace SPFT.PowerUpSystem.PowerUps {

    public class PuckBounceMod : PowerUpBase {

        private const int INIT_ARG_COUNT = 4;

        // Initialization Arg Names
        private const string PUCK_BOUNCE_MOD = "puckBounceMod";
        private const string PWR_UP_DURATION = "pwrUpDuration";

        // Fields that should be set during Initialization.
        public float puckBounceMod;
        public float pwrUpDuration;

        private PhysicsMaterial2D puckMaterial;
        private float originalPuckBounce;

        private PowerUpTimer powerUpTimer;
        private Guid timerId;

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
                        Debug.LogWarning($"No parameter found for {arg.name}");
                        break;
                }
            }
        }

        void Start() {
            puckMaterial = PowerUpLifeCycleManager.Instance.puck.GetComponent<PhysicsMaterial2D>();
            EventManager.Instance.RegisterListener<PowerUpTimerExpiredEvent>(OnPowerUpTimerExpiration);
        }

        public override void Activate() {
            Debug.Log($"Activating {GetType()} PowerUp.");
            IsActive = true;

            originalPuckBounce = puckMaterial.bounciness;
            puckMaterial.bounciness = puckBounceMod;

            timerId = PowerUpTimer.Instance.StartTimer(pwrUpDuration);
        }

        public override void Deactivate() {
            Debug.Log($"Deactivating {GetType()} PowerUp.");
            IsActive = false;

            puckMaterial.bounciness = originalPuckBounce;

            EmitExpiredEventAndSelfDestruct(this, gameSettings.pwrUpPostDeactivationDelay);
        }

        public void OnPowerUpTimerExpiration(PowerUpTimerExpiredEvent powerUpTimerExpiredEvent) {
            if (timerId != null && timerId.Equals(powerUpTimerExpiredEvent.timerId)) {
                Deactivate();
            }
        }
    }
}
