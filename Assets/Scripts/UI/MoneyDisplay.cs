using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoneyDisplay : SingletonBase<ScoreDisplay> {

    private const string PLAYER_CASH_FORMAT = "$ {0}";

    private Text moneyDisplayText;

    void Start() {
        moneyDisplayText = GetComponent<Text>();
    }

    public void UpdateMoneyDisplay(int currentCashBalance) {
        moneyDisplayText.text = string.Format(PLAYER_CASH_FORMAT, currentCashBalance);
    }
}
