using System;
using UnityEngine;
using UnityEngine.UI;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;
using SPFT.State;

public class PlayerWallet : MonoBehaviour {

    private static PlayerWallet wallet;
    public static PlayerWallet Instance {
        get {
            if (!wallet) {
                wallet = FindObjectOfType(typeof(PlayerWallet)) as PlayerWallet;
                if (!wallet) {
                    Debug.LogError("There needs to be one active PlayerWallet script on a GameObject in your scene.");
                } else {
                    wallet.Init();
                }
            }
            return wallet;
        }
    }

    private const string BALANCE_PREFIX = "CASH: $";

    private int totalBalance;
    private GameSettings gameSettings;

    public GameObject goalAreas;
    private Goal[] goals;

    public Puck puck;
    public Text totalBalanceText;

    void Init() {
        UpdateScoreDisplay();
    }

    void Awake() {
        goals = goalAreas.GetComponentsInChildren<Goal>();
        if (goals == null || goals.Length == 0) {
            throw new InvalidOperationException("There must be at least one Goal in GoalAreas.");
        }
    }

    void Start() {
        gameSettings = GameSettings.Instance;
        totalBalance = gameSettings.startingCash;

        EventManager.Instance.RegisterListener<BuyPuckEvent>(OnPuckPurchase);
        EventManager.Instance.RegisterListener<PuckScoreEvent>(OnPuckScore);
        EventManager.Instance.RegisterListener<KillPuckEvent>(OnPuckDeath);
    }

    void Update() {
        UpdateScoreDisplay();
        MainGameState gameState = GameStateManager.Instance.State;
        if (gameState != MainGameState.GAME_PAUSED && Input.GetButtonDown(gameSettings.buyPowerUp)) {
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
