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

    [SerializeField]
    public IList<GameObject> powerUpMasterList;
    public IDictionary<Guid, Sprite> powerUpIconsByGuid;

    private GameSettings gameSettings;
    private PlayerWallet wallet;
    private PowerUpManager powerUpMgr;

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
    }

    void Start() {
        powerUpMasterList = DataLoader.Instance.LoadPowerUpData();
        if (powerUpMasterList == null || powerUpMasterList.Count == 0) {
            throw new InvalidOperationException("There must be at least one PowerUp in allPowerUps.");
        }

        //if (powerUpIconsByGuid.Count != PowerUpManager.Instance.allPowerUpPrefabs.Count) {
        //    throw new InvalidOperationException("There must be one Sprite for each PowerUp.");
        //}
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

        // Select and instantiate a random PowerUp from the master list.
        GameObject powerUpTemplate = powerUpMasterList[Random.Range(0, powerUpMasterList.Count)];
        IPowerUp pwrUp = Instantiate(powerUpTemplate, Vector3.zero, Quaternion.identity).GetComponent<IPowerUp>();

        // Charge the player for the cost of a PowerUp.
        wallet.ChargePlayer(gameSettings.powerUpCost);

        Debug.Log($"Successfully bought the {pwrUp.GetType().ToString()} PowerUp Type.");

        // Notify listeners that a PowerUp has been acquired.
        PowerUpAcquiredEvent powerUpAcquiredEvent = new PowerUpAcquiredEvent() {
            powerUp = pwrUp,
            activationType = PowerUpActivationType.MANUAL
        };
        EventManager.Instance.NotifyListeners(powerUpAcquiredEvent);
    }

}
