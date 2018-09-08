using UnityEngine;
using System;
using System.Collections.Generic;
using static PowerUpAcquiredPayload;

public class StoreManager : MonoBehaviour {

    public const float DEFAULT_COST_PER_PWR_UP = 25f;
    public const string BUY_RAND_PWR_UP_INPUT = "space";

    private static StoreManager storeMgr;
    public static StoreManager Instance {
        get {
            if (!storeMgr) {
                storeMgr = FindObjectOfType(typeof(StoreManager)) as StoreManager;
                if (!storeMgr) {
                    Debug.LogError("There needs to be one active StoreManager script on a GameObject in your scene.");
                } else {
                    storeMgr.Init();
                }
            }
            return storeMgr;
        }
    }

    public float Cash { get; private set; }
    public bool IsRandomizingPowerUps { get; private set; }

    public float costPerPwrUp;
    public Dictionary<Guid, Sprite> powerUpIconsByGuid;
    public PowerUpAcquiredEvent pwrUpAcquiredEvent;

    void Init() {
        // what should go here?
    }

    void Awake() {
        if (powerUpIconsByGuid == null || powerUpIconsByGuid.Count == 0) {
            throw new InvalidOperationException("There must be at least one Sprite in powerUpIcons.");
        }
        if (costPerPwrUp == 0f) {
            costPerPwrUp = DEFAULT_COST_PER_PWR_UP;
        }
        Cash = 0f;
        IsRandomizingPowerUps = false;
        pwrUpAcquiredEvent = new PowerUpAcquiredEvent();
    }

    void Start() {
        if (powerUpIconsByGuid.Count != PowerUpManager.Instance.allPowerUpPrefabs.Count) {
            throw new InvalidOperationException("There must be one Sprite for each PowerUp.");
        }
    }

    void Update() {
        if (Input.GetKeyDown(BUY_RAND_PWR_UP_INPUT)) {
            if (!IsRandomizingPowerUps && Cash >= costPerPwrUp) {
                SpinPowerUpSlotMachine();
            } else if (IsRandomizingPowerUps) {
                StopPowerUpSlotMachine();
            }
        }
    }

    private void SpinPowerUpSlotMachine() {
        Cash -= costPerPwrUp;
        IsRandomizingPowerUps = true;
        // spin slot machine with powerup icons
    }

    private void StopPowerUpSlotMachine() {
        IsRandomizingPowerUps = false;
        IPowerUp randomPowerUp = PowerUpManager.Instance.GetRandomPowerUp();
        Sprite powerUpIcon = powerUpIconsByGuid[randomPowerUp.Id];
        // stop slot reel on the given icon

        // Notify listeners that we have acquired a new payload, this will activate the PowerUp
        PowerUpAcquiredPayload pwrUpAcquiredPayload = new PowerUpAcquiredPayload(randomPowerUp, ActivationType.IMMEDIATE);
        pwrUpAcquiredEvent.Invoke(pwrUpAcquiredPayload);
    }

}
