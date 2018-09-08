using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;
using SPFT.PowerUpSystem.PowerUps;


namespace SPFT.PowerUpSystem {

    public class PowerUpManager : MonoBehaviour {

        private static PowerUpManager powerUpMgr;
        public static PowerUpManager Instance {
            get {
                if (!powerUpMgr) {
                    powerUpMgr = FindObjectOfType(typeof(PowerUpManager)) as PowerUpManager;
                    if (!powerUpMgr) {
                        Debug.LogError("There needs to be one active PowerUpManager script on a GameObject in your scene.");
                    } else {
                        powerUpMgr.Init();
                    }
                }
                return powerUpMgr;
            }
        }

        public Puck puck;
        public Text timerText;
        public int slotMachineCost;

        public PowerUpAcquiredEvent pwrUpAcquiredEvent;

        [SerializeField]
        public Queue<IPowerUp> storedPowerUps;
        [SerializeField]
        public List<IPowerUp> activePowerUps;
        [SerializeField]
        public List<IPowerUp> expiredPowerUps;

        private GameSettings gameSettings;

        void Init() {
            if (puck == null) {
                throw new InvalidOperationException("Puck cannot be null.");
            }
            UpdateTimerDisplay(0f);
        }

        void Awake() {
            storedPowerUps = new Queue<IPowerUp>();
            activePowerUps = new List<IPowerUp>();
            expiredPowerUps = new List<IPowerUp>();
        }

        void Start() {
            gameSettings = GameSettings.Instance;
            EventManager.Instance.RegisterListener<PowerUpAcquiredEvent>(OnPowerUpAcquired);
        }

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

        public void DeactivatePowerUp(IPowerUp powerUp) {
            powerUp.Deactivate();
            activePowerUps.Remove(powerUp);
            expiredPowerUps.Add(powerUp);

            // Notify all the listeners that the PowerUp has been Deactivated
            PowerUpExpiredEvent powerUpExpiredEvent = new PowerUpExpiredEvent() {
                powerUp = powerUp
            };
            EventManager.Instance.NotifyListeners(powerUpExpiredEvent);
        }

        public void UpdateTimerDisplay(float seconds) {
            int min = (int) (seconds / 60f);
            int sec = (int) (seconds % 60f);
            timerText.text = $"{min}:{sec}";
        }

        /* ~~~~~~~~~~~~~~~~~~~~ Unity Event Handlers ~~~~~~~~~~~~~~~~~~~~ */

        public void OnPowerUpAcquired(PowerUpAcquiredEvent powerUpAcquiredEvent) {
            if (powerUpAcquiredEvent.activationType == PowerUpActivationType.IMMEDIATE) {
                ActivatePowerUp(powerUpAcquiredEvent.powerUp);
            } else {
                Debug.Log("Enqueuing newly acquired PowerUp.");
                storedPowerUps.Enqueue(powerUpAcquiredEvent.powerUp);
            }
        }
    }
}
