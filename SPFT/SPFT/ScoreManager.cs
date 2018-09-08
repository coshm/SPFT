using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;

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

    private const string SCORE_PREFIX = "Score: $";

    public List<Goal> goals;

    public Puck puck;
    public Text scoreText;

    private int totalScore;

    void Init() {
        UpdateScoreDisplay();
    }

    void Awake() {
        if (goals == null || goals.Count == 0) {
            throw new InvalidOperationException("Goals must be set in the editor.");
        }
    }

    void Start() {
        EventManager.Instance.RegisterListener<ScoreEvent>(OnScore);
    }

    void Update() {
        
    }

    public void OnScore(ScoreEvent scoreEvent) {
        totalScore += scoreEvent.score;
        UpdateScoreDisplay();
    }

    public bool SpendScore(int scoreToSpend) {
        if (scoreToSpend <= totalScore) {
            totalScore -= scoreToSpend;
            UpdateScoreDisplay();
            return true;
        }
        return false;
    }

    public bool CanAffordSpending(int scoreToSpend) {
        return scoreToSpend <= totalScore;
    }

    public void UpdateGoalScores(int idx, int newScore) {
        goals[idx].ChangeScore(newScore);
    }

    public void MultiplyAllGoalScores(float modifier) {
        foreach (Goal goal in goals) {
            goal.ChangeScore((int) (goal.score * modifier));
        }
    }

    private void UpdateScoreDisplay() {
        scoreText.text = SCORE_PREFIX + totalScore;
    }
}
