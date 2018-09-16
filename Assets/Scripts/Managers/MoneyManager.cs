using System;
using UnityEngine;
using UnityEngine.UI;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;
using SPFT.State;

public class MoneyManager : SingletonBase<PlayerWallet> {

    private GameSettings gameSettings;
    private GameStateManager gameState;
    private MoneyDisplay moneyDisplay;

    private int cashBalance;

    void Awake() {

    }

    void Start() {
        gameSettings = GameSettings.Instance;
        gameState = GameStateManager.Instance;

        cashBalance = gameSettings.startingCash;

        EventManager.Instance.RegisterListener<BuyPuckEvent>(OnPuckPurchase);
        EventManager.Instance.RegisterListener<PuckScoreEvent>(OnPuckScore);
        EventManager.Instance.RegisterListener<KillPuckEvent>(OnPuckDeath);
    }

    void Update() {
        MainGameState gameState = GameStateManager.Instance.State;
        if (!gameState.IsGamePaused() && Input.GetButtonDown(gameSettings.buyPowerUp)) {
            if (CanAffordCharge(gameSettings.powerUpCost)) {
                ChargePlayer(gameSettings.powerUpCost);
                EventManager.Instance.NotifyListeners(new BuyPowerUpEvent());
            }
        }
    }

    public void OnPuckPurchase(BuyPuckEvent buyPuckEvent) {
        ChargePlayer(gameSettings.puckCost);
    }

    public void OnPuckScore(PuckScoreEvent puckScoreEvent) {
        totalBalance += puckScoreEvent.cashPrize;
        UpdateScoreDisplay();
    }

    public void OnPuckDeath(KillPuckEvent killPuckEvent) {
        if (killPuckEvent.causeOfDeath == CauseOfDeath.OUT_OF_BOUNDS) {
            ChargePlayer(killPuckEvent.cashPenalty);
        }
    }

    public bool CanAffordCharge(int chargeAmount) {
        return totalBalance > chargeAmount;
    }

    public void ChargePlayer(int chargeAmount) {
        totalBalance -= chargeAmount;
        if (totalBalance <= 0) {
            GameOverEvent gameOverEvent = new GameOverEvent() {
                causeOfGameOver = CauseOfGameOver.BANKRUPT
            };
            EventManager.Instance.NotifyListeners(gameOverEvent);
        }
    }
    
    private void UpdateScoreDisplay() {
        totalBalanceText.text = BALANCE_PREFIX + totalBalance;
    }
}
