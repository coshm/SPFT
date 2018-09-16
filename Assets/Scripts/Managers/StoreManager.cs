using UnityEngine;
using SPFT.PowerUpSystem;

public class StoreManager : SingletonBase<StoreManager> {

    public bool IsBuyingPowerUp { get; private set; }

    private GameSettings gameSettings;
    private PlayerWallet wallet;
    private PowerUpLifeCycleManager pwrUpLifeCycleMgr;
    private SlotMachine pwrUpSlotMachine;

    void Awake() {
        IsBuyingPowerUp = false;
        gameSettings = GameSettings.Instance;
        wallet = PlayerWallet.Instance;
        pwrUpLifeCycleMgr = PowerUpLifeCycleManager.Instance;
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
        SlotMachine.Instance.PlaySlotsForPowerUp();

        // Charge the player for the cost of a PowerUp.
        wallet.ChargePlayer(gameSettings.powerUpCost);
    }

}
