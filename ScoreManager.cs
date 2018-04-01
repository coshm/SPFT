using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    private static ScoreManager scoreMgr;
    public static ScoreManager Instance {
        get {
            if (!scoreMgr) {
                scoreMgr = FindObjectOfType(typeof(ScoreManager)) as ScoreManager;
                if (!scoreMgr) {
                    Debug.LogError("There needs to be one active ScoreManager script on a GameObject in your scene.");
                } else {
                    scoreMgr.Init();
                }
            }
            return scoreMgr;
        }
    }

    private const string SCORE_PREFIX = "Score: ";

    public Puck puck;
    public Text scoreText;
    private int score;

    void Init() {
        UpdateScoreDisplay();
    }

    void Awake() {

    }

    void Start() {
        EventManager.Instance.RegisterListenerWithPayload<ScoreEvent>(OnScore);
    }

    void Update() {

    }

    public void OnScore(IEventPayload genericPayload) {
        if (genericPayload.GetType() == typeof(ScorePayload)) {
            ScorePayload scorePayload = (ScorePayload)genericPayload;
            score += scorePayload.Score;
            UpdateScoreDisplay();
        }
    }

    private void UpdateScoreDisplay() {
        scoreText.text = SCORE_PREFIX + score;
    }
}
