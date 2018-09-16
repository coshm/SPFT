using Random = UnityEngine.Random;
using UnityEngine;
using System;
using System.Collections.Generic;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;
using SPFT.PowerUpSystem;
using SPFT.PowerUpSystem.PowerUps;

public class StoreManager : MonoBehaviour {

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
    
    public bool IsBuyingPowerUp { get; private set; }

    private GameSettings gameSettings;
    private PlayerWallet wallet;
    private PowerUpManager powerUpMgr;
    private SlotMachine pwrUpSlotMachine;

    void Init() {
        // what should go here?
    }

    void Awake() {
        //if (powerUpIconsByGuid == null || powerUpIconsByGuid.Count == 0) {
        //    throw new InvalidOperationException("There must be at least one Sprite in powerUpIcons.");
        //}
        IsBuyingPowerUp = false;
        gameSettings = GameSettings.Instance;
        wallet = PlayerWallet.Instance;
        powerUpMgr = PowerUpManager.Instance;
        pwrUpSlotMachine = SlotMachine.Instance;
    }

    void Start() {
    }

    void Update() {
        if (Input.GetButtonDown(gameSettings.buyPowerUp)) {
            bool canAffordPwrUp = wallet.CanAffordCharge(gameSettings.powerUpCost);
            if (!IsBuyingPowerUp && canAffordPwrUp) {
                BuyRandomPowerUp();
            }
        }
    }

    private void BuyRandomPowerUp() {
        // kick off slot machine animation
        GameObject selectedPowerUpPrefab = pwrUpSlotMachine.PullSlotMachine();

        // Select and instantiate a random PowerUp from the master list.
        //GameObject powerUpTemplate = powerUpMasterList[Random.Range(0, powerUpMasterList.Count)];
        IPowerUp powerUp = Instantiate(selectedPowerUpPrefab, Vector3.zero, Quaternion.identity).GetComponent<IPowerUp>();

        // Charge the player for the cost of a PowerUp.
        wallet.ChargePlayer(gameSettings.powerUpCost);

        Debug.Log($"Successfully bought the {powerUp.GetType().ToString()} PowerUp Type.");

        // Notify listeners that a PowerUp has been acquired.
        PowerUpAcquiredEvent powerUpAcquiredEvent = new PowerUpAcquiredEvent() {
            powerUp = powerUp,
            activationType = PowerUpActivationType.MANUAL
        };
        EventManager.Instance.NotifyListeners(powerUpAcquiredEvent);
    }

}
