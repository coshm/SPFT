using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;
using SPFT.PowerUpSystem.PowerUps;

namespace SPFT.PowerUpSystem {

    public class PowerUpLifeCycleManager : SingletonBase<PowerUpLifeCycleManager> {

        public Puck puck;

        [SerializeField]
        private Queue<IPowerUp> storedPowerUps;
        public int StoredPowerUpCount { get { return storedPowerUps.Count; } }

        [SerializeField]
        public List<IPowerUp> activePowerUps;
        [SerializeField]
        public List<IPowerUp> expiredPowerUps;

        private GameSettings gameSettings;
        private PowerUpTimer pwrUpTimer;
        private StoredPowerUpWidget storedPowerUpWidget;

        void Init() {
            if (puck == null) {
                throw new InvalidOperationException("Both Puck and PowerUpTimer are required.");
            }
        }

        void Awake() {
            gameSettings = GameSettings.Instance;
            pwrUpTimer = PowerUpTimer.Instance;
            storedPowerUpWidget = StoredPowerUpWidget.Instance;

            storedPowerUps = new Queue<IPowerUp>();
            activePowerUps = new List<IPowerUp>();
            expiredPowerUps = new List<IPowerUp>();
        }

        void Start() {
            EventManager.Instance.RegisterListener<PowerUpAcquiredEvent>(OnPowerUpAcquisition);
            EventManager.Instance.RegisterListener<PowerUpExpiredEvent>(OnPowerUpExpiration);
        }

        /* ~~~~~~~~~~~~~~~~~~~~ Manage PowerUp Lifecycle ~~~~~~~~~~~~~~~~~~~~ */

        void Update() {
            if (Input.GetButtonDown(gameSettings.activatePowerUp)) {
                HandleActivatePowerUpAttempt();
            }
        }

        public void HandleActivatePowerUpAttempt() {
            // If we don't have any stored PowerUps, then nothing to do
            if (storedPowerUps.Count == 0) {
                Debug.Log("No Stored PowerUps");
                return;
            }

            // Check if any active PowerUps are blocking the activation of
            //  the next PowerUp before removing it from storedPowerUps
            IPowerUp nextPwrUp = storedPowerUps.Peek();
            if (!nextPwrUp.IsBlocked(activePowerUps)) {
                storedPowerUps.Dequeue();
                storedPowerUpWidget.UpdateStoredPowerUpIcons(storedPowerUps);
                ActivatePowerUp(nextPwrUp);
            } else {
                // Display a warning saying we're blocked
            }
        }

        public void ActivatePowerUp(IPowerUp powerUp) {
            powerUp.Activate();
            activePowerUps.Add(powerUp);

            // Notify all the listeners that the PowerUp has been Activated
            PowerUpActivatedEvent powerUpActivatedEvent = new PowerUpActivatedEvent() {
                powerUp = powerUp
            };
            EventManager.Instance.NotifyListeners(powerUpActivatedEvent);
        }

        /* ~~~~~~~~~~~~~~~~~~~~ Stored PowerUp Icon Helpers ~~~~~~~~~~~~~~~~~~~~ */

        public void SetStoredPowerUpOrder(int[] reorderedStoredPwrUpIndices) {
            Queue<IPowerUp> reorderdStoredPowerUps = new Queue<IPowerUp>();
            foreach (int storedPowerUpIdx in reorderedStoredPwrUpIndices) {
                reorderdStoredPowerUps.Enqueue(storedPowerUps.ElementAt(storedPowerUpIdx));
            }
            storedPowerUps = reorderdStoredPowerUps;
            storedPowerUpWidget.UpdateStoredPowerUpIcons(storedPowerUps);
        }

        /* ~~~~~~~~~~~~~~~~~~~~ Unity Event Handlers ~~~~~~~~~~~~~~~~~~~~ */

        public void OnPowerUpAcquisition(PowerUpAcquiredEvent powerUpAcquiredEvent) {
            if (powerUpAcquiredEvent.activationType == PowerUpActivationType.IMMEDIATE) {
                ActivatePowerUp(powerUpAcquiredEvent.powerUp);
            } else {
                Debug.Log("Enqueuing newly acquired PowerUp.");
                storedPowerUps.Enqueue(powerUpAcquiredEvent.powerUp);
                storedPowerUpWidget.UpdateStoredPowerUpIcons(storedPowerUps);
            }
        }

        public void OnPowerUpExpiration(PowerUpExpiredEvent powerUpExpiredEvent) {
            activePowerUps.Remove(powerUpExpiredEvent.powerUp);
            expiredPowerUps.Add(powerUpExpiredEvent.powerUp);
        }
    }
}
