using Random = UnityEngine.Random;
using ActivationType = PowerUpAcquiredPayload.ActivationType;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour {

    private const string BUY_PWR_UP_KEY = "space";
    private const string ACTIVATE_PWR_UP_KEY = "a";
    private const int DEFAULT_SLOT_MACHINE_COST = 100;
    private const int MAX_STORED_POWER_UPS = 5;

    public const string ICON_TAG_PREFIX = "StoredPowerUpIcon";
    public const string ICON_TAG_DELIMETER = "_";

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

    public int StoredPowerUpCount => storedPowerUps.Count();

    public Puck puck;
    public Text timerText;
    public int slotMachineCost;

    public Dictionary<int, SpriteRenderer> storedPowerUpIconSlots;

    [SerializeField]
    public List<GameObject> allPowerUpPrefabs;
    [SerializeField]
    public Queue<IPowerUp> storedPowerUps;
    [SerializeField]
    public List<IPowerUp> activePowerUps;
    [SerializeField]
    public List<IPowerUp> expiredPowerUps;

    private ScoreManager scoreMgr;

    void Init() {
        if (puck == null) {
            throw new InvalidOperationException("Puck cannot be null.");
        }
        scoreMgr = ScoreManager.Instance;
        UpdateTimerDisplay(0f);
    }

    void Awake() {
        if (slotMachineCost == 0) {
            slotMachineCost = DEFAULT_SLOT_MACHINE_COST;
        }

        if (allPowerUpPrefabs == null || allPowerUpPrefabs.Count == 0) {
            throw new InvalidOperationException("There must be at least one PowerUp in allPowerUps.");
        }

        storedPowerUpIconSlots = new Dictionary<int, SpriteRenderer>();
        IList<SpriteRenderer> iconSlots = GetComponentsInChildren<SpriteRenderer>();
        if (iconSlots == null || iconSlots.Count != MAX_STORED_POWER_UPS) {
            throw new InvalidOperationException($"Only found {iconSlots.Count} icon slots, not {MAX_STORED_POWER_UPS}");
        }

        foreach (SpriteRenderer iconSlot in iconSlots) {
            int idx = GetIndexFromIconTag(iconSlot.gameObject);
            storedPowerUpIconSlots[idx] = iconSlot;
        }

        storedPowerUps = new Queue<IPowerUp>();
        activePowerUps = new List<IPowerUp>();
        expiredPowerUps = new List<IPowerUp>();
    }

    void Start() {
        EventManager.Instance.RegisterListenerWithPayload<PowerUpAcquiredEvent>(OnPowerUpAcquired);
        EventManager.Instance.RegisterListenerWithPayload<PowerUpExpiredEvent>(OnPowerUpExpired);
    }

    void Update() {
        if (Input.GetButtonDown(BUY_PWR_UP_KEY)) {
            GetRandomPowerUp();
        }else if (Input.GetButtonDown(ACTIVATE_PWR_UP_KEY)) {
            ActivateStoredPowerUp();
        }
    }

    public void ActivateStoredPowerUp() {
        // If we don't have any stored PowerUps, then nothing to do
        if (storedPowerUps.Count == 0) {
            Debug.Log("No Stored PowerUps");
            return;
        }

        // Check if any active PowerUps are blocking the 
        // activation of the next PowerUp
        IPowerUp nextPwrUp = storedPowerUps.Peek();
        bool isBlocked = false;
        activePowerUps.ForEach(delegate (IPowerUp pwrUp) {
            isBlocked |= pwrUp.IsBlockingPowerUpActivation(nextPwrUp);
        });

        // If next PowerUp is not blocked, we can activate  
        // it and remove it from the stored PowerUp queue
        if (!isBlocked) {
            storedPowerUps.Dequeue();
            UpdateStoredPowerUpIcons();
            ActivatePowerUp(nextPwrUp);
        }
    }

    public void ActivatePowerUp(IPowerUp powerUp) {
        powerUp.Activate();
        activePowerUps.Add(powerUp);
    }

    public void DeactivatePowerUp(IPowerUp powerUp) {
        powerUp.Deactivate();
        activePowerUps.Remove(powerUp);
        expiredPowerUps.Add(powerUp);
    }

    public IPowerUp GetRandomPowerUp() {
        // Get random PowerUp prefab and instantiate it
        GameObject powerUpPrefab = allPowerUpPrefabs[Random.Range(0, allPowerUpPrefabs.Count)];
        return Instantiate(powerUpPrefab, Vector3.zero, Quaternion.identity).GetComponent<IPowerUp>();
    }

    public void UpdateTimerDisplay(float seconds) {
        int min = (int) (seconds / 60f);
        int sec = (int) (seconds % 60f);
        timerText.text = $"{min}:{sec}";
    }

    /* ~~~~~~~~~~~~~~~~~~~~ Stored PowerUp Icon Helpers ~~~~~~~~~~~~~~~~~~~~ */

    public bool IsAPowerUpIcon(GameObject obj) {
        return obj.tag.startsWith(ICON_TAG_PREFIX);
    }

    public int GetIndexFromIconTag(GameObject iconObj) {
        return int.Parse(iconObj.tag.split(ICON_TAG_DELIMETER)[1]);
    }

    public void SetStoredPowerUpOrder(int[] reorderedStoredPwrUpIndices) {
        Queue<IPowerUp> reorderdStoredPowerUps = new Queue<IPowerUp>();
        foreach (int storedPowerUpIdx in reorderedStoredPwrUpIndices) {
            reorderdStoredPowerUps.enqueue(storedPowerUps.ElementAt(storedPowerUpIdx));
        }
        storedPowerUps = reorderdStoredPowerUps;
        UpdateStoredPowerUpIcons();
    }

    private void UpdateStoredPowerUpIcons() {
        for (int i = 0; i < MAX_STORED_POWER_UPS; i++) {
            storedPowerUpIconSlots[i].Sprite = storedPowerUps.ElementAt(i).Icon;
            if (i >= StoredPowerUpCount) {
                storedPowerUpIconSlots[i].Sprite = null;
            }
        }
    }

    /* ~~~~~~~~~~~~~~~~~~~~ Unity Event Handlers ~~~~~~~~~~~~~~~~~~~~ */

    public void OnPowerUpAcquired(IEventPayload genericPayload) {
        if (genericPayload.GetType() == typeof(PowerUpAcquiredPayload)) {
            PowerUpAcquiredPayload pwrUpAcquiredPayload = (PowerUpAcquiredPayload)genericPayload;
            if (pwrUpAcquiredPayload.Type == ActivationType.IMMEDIATE) {
                ActivatePowerUp(pwrUpAcquiredPayload.PowerUp);
            } else {
                storedPowerUps.Enqueue(pwrUpAcquiredPayload.PowerUp);
                UpdateStoredPowerUpIcons();
            }
        }
    }

    public void OnPowerUpExpired(IEventPayload genericPayload) {
        if (genericPayload.GetType() == typeof(PowerUpExpiredPayload)) {
            PowerUpExpiredPayload pwrUpExpiredPayload = (PowerUpExpiredPayload)genericPayload;
            activePowerUps.Remove(pwrUpExpiredPayload.PowerUp);
        }
    }

    public void OnCancelActivePowerUp() {
        if (activePowerUps.Count > 0) {
            DeactivatePowerUp(activePowerUps[0]);
        }
    }

    public void OnSlotMachinePull() {
        if (scoreMgr.CanAffordSpending(slotMachineCost)) {
            IPowerUp powerUpPrize = GetRandomPowerUp();
            storedPowerUps.Enqueue(powerUpPrize);
            scoreMgr.SpendScore(slotMachineCost);
        }
    }

    /* ~~~~~~~~~~~~~~~~~~ PowerUp Trigger Handlers ~~~~~~~~~~~~~~~~~~ */

    public bool OnPowerUpTrigger(IPowerUpTrigger pwrUpTrigger) {
        bool wasHandled = false;
        activePowerUps.ForEach(delegate (IPowerUp pwrUp) {
            wasHandled |= pwrUp.OnPowerUpTrigger(pwrUpTrigger);
        });
        return wasHandled;
    }
}
