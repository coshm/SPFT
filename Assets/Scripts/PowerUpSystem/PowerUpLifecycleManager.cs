using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;
using SPFT.PowerUpSystem.PowerUps;

namespace SPFT.PowerUpSystem {

    public class PowerUpLifeCycleManager : MonoBehaviour {

        public const string ICON_TAG_PREFIX = "StoredPowerUpIcon";
        public const char ICON_TAG_DELIMETER = '_';

        private static PowerUpLifeCycleManager pwrUpLifeCycleMgr;
        public static PowerUpLifeCycleManager Instance {
            get {
                if (!pwrUpLifeCycleMgr) {
                    pwrUpLifeCycleMgr = FindObjectOfType(typeof(PowerUpLifeCycleManager)) as PowerUpLifeCycleManager;
                    if (!pwrUpLifeCycleMgr) {
                        Debug.LogError($"There needs to be one active {GetType()} script on a GameObject in your scene.");
                    } else {
                        pwrUpLifeCycleMgr.Init();
                    }
                }
                return pwrUpLifeCycleMgr;
            }
        }

        public Puck puck;
        public PowerUpTimer pwrUpTimer;

        [SerializeField]
        private Queue<IPowerUp> storedPowerUps;
        public int StoredPowerUpCount { get { return storedPowerUps.Count; } }
        public Dictionary<int, SpriteRenderer> storedPowerUpIconSlots;
        public Sprite emptyPowerUpIconSlot;

        [SerializeField]
        public List<IPowerUp> activePowerUps;
        [SerializeField]
        public List<IPowerUp> expiredPowerUps;

        private GameSettings gameSettings;

        void Init() {
            if (puck == null || pwrUpTimer == null) {
                throw new InvalidOperationException("Both Puck and PowerUpTimer are required.");
            }
        }

        void Awake() {
            gameSettings = GameSettings.Instance;

            // Get the SpriteRenderers from all child GOs, which are all the icon slots for store powerUps.
            storedPowerUpIconSlots = new Dictionary<int, SpriteRenderer>();
            IList<SpriteRenderer> iconSlots = GetComponentsInChildren<SpriteRenderer>();
            if (iconSlots == null || iconSlots.Count != gameSettings.maxStoredPowerUps) {
                throw new InvalidOperationException($"Only found {iconSlots.Count} icon slots, not the expected {gameSettings.maxStoredPowerUps}");
            }

            // Initialize each iconSlot with by setting the empty iconSlot sprite and getting their indices.
            emptyPowerUpIconSlot = SpriteHelper.GetSprite(SpriteHelper.POWER_UP_ICONS, 6);
            foreach (SpriteRenderer iconSlot in iconSlots) {
                int idx = GetIndexFromIconTag(iconSlot.gameObject);
                iconSlot.sprite = emptyPowerUpIconSlot;
                storedPowerUpIconSlots[idx] = iconSlot;
            }

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
                UpdateStoredPowerUpIcons();
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

        public bool IsAPowerUpIcon(GameObject obj) {
            return obj.tag.StartsWith(ICON_TAG_PREFIX);
        }

        public int GetIndexFromIconTag(GameObject iconObj) {
            Debug.Log($"Getting Index from IconTag={iconObj.tag}");
            return int.Parse(iconObj.tag.Split(ICON_TAG_DELIMETER)[1]);
        }

        public void SetStoredPowerUpOrder(int[] reorderedStoredPwrUpIndices) {
            Queue<IPowerUp> reorderdStoredPowerUps = new Queue<IPowerUp>();
            foreach (int storedPowerUpIdx in reorderedStoredPwrUpIndices) {
                reorderdStoredPowerUps.Enqueue(storedPowerUps.ElementAt(storedPowerUpIdx));
            }
            storedPowerUps = reorderdStoredPowerUps;
            UpdateStoredPowerUpIcons();
        }

        private void UpdateStoredPowerUpIcons() {
            for (int i = 0; i < storedPowerUpIconSlots.Count; i++) {

                if (i >= StoredPowerUpCount) {
                    storedPowerUpIconSlots[i].sprite = storedPowerUps.ElementAt(i).Icon;
                } else {
                    storedPowerUpIconSlots[i].sprite = null;
                }

            }
        }

        /* ~~~~~~~~~~~~~~~~~~~~ Unity Event Handlers ~~~~~~~~~~~~~~~~~~~~ */

        public void OnPowerUpAcquisition(PowerUpAcquiredEvent powerUpAcquiredEvent) {
            if (powerUpAcquiredEvent.activationType == PowerUpActivationType.IMMEDIATE) {
                ActivatePowerUp(powerUpAcquiredEvent.powerUp);
            } else {
                Debug.Log("Enqueuing newly acquired PowerUp.");
                storedPowerUps.Enqueue(powerUpAcquiredEvent.powerUp);
                UpdateStoredPowerUpIcons();
            }
        }

        public void OnPowerUpExpiration(PowerUpExpiredEvent powerUpExpiredEvent) {
            activePowerUps.Remove(powerUp);
            expiredPowerUps.Add(powerUp);
        }
    }
}
