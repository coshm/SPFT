using UnityEngine;
using System;
using SPFT.EventSystem.Events;

namespace SPFT.PowerUpSystem.PowerUps {

    public class GoalScoreMod : PowerUpBase {

        private const int INIT_ARG_COUNT = 4;

        // Initialization Arg Names
        private const string SCORE_MULTIPLIER = "scoreMultiplier";
        private const string SCORE_MOD_DURATION = "scoreModDuration";
        private const string PWR_UP_DURATION = "pwrUpDuration";

        public float scoreMultiplier;
        public float scoreModDuration;
        public float powerUpDuration;

        private GoalManager goalMgr;
        private Guid timerId;

        public override void Initialize(params PowerUpArg[] args) {
            InitializeBase(args);
            if (args.Length != INIT_ARG_COUNT) {
                throw new InvalidOperationException($"Expected {INIT_ARG_COUNT} init args but actually got {args.Length}.");
            }

            foreach (PowerUpArg arg in args) {
                switch (arg.name) {
                    case SCORE_MULTIPLIER:
                        scoreMultiplier = Utilities.ConvertStringOrDefault(arg.value, gameSettings.scoreMultiplier);
                        break;
                    case SCORE_MOD_DURATION:
                        scoreModDuration = Utilities.ConvertStringOrDefault(arg.value, gameSettings.scoreModDuration);
                        break;
                    case PWR_UP_DURATION:
                        powerUpDuration = Utilities.ConvertStringOrDefault(arg.value, gameSettings.powerUpDuration);
                        break;
                    default:
                        Debug.LogWarning($"No parameter found for {arg.name}");
                        break;
                }
            }
        }

        void Awake() {
            goalMgr = GoalManager.Instance;
        }

        public override void Activate() {
            Debug.Log($"Activating {GetType()} PowerUp.");
            IsActive = true;

            goalMgr.MultipleGoalScores(scoreMultiplier);

            // Start timer for this PowerUps lifecycle
            timerId = PowerUpTimer.Instance.StartTimer(powerUpDuration);
        }

        public override void Deactivate() {
            Debug.Log($"Deactivating {GetType()} PowerUp."); 
            IsActive = false;

            goalMgr.ResetGoalScores();

            EmitExpiredEventAndSelfDestruct(this, gameSettings.pwrUpPostDeactivationDelay);
        }

        public void OnPowerUpTimerExpiration(PowerUpTimerExpiredEvent powerUpTimerExpiredEvent) {
            if (timerId != null && timerId.Equals(powerUpTimerExpiredEvent.timerId)) {
                Deactivate();
            }
        }
    }
}