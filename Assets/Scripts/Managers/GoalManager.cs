using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour {

    private static GoalManager goalManager;
    public static GoalManager Instance {
        get {
            if (!goalManager) {
                goalManager = FindObjectOfType(typeof(GoalManager)) as GoalManager;
                if (!goalManager) {
                    Debug.LogError("There needs to be one active GoalManager script on a GameObject in your scene.");
                }
            }
            return goalManager;
        }
    }

    private IList<Goal> allGoals;

    [SerializeField]
    private float firstX;
    [SerializeField]
    private float xOffset;
    [SerializeField]
    private float goalY;

    public int GoalCount { get; private set; }
    public int[] GoalIndices { get; private set; }

    // Use this for initialization
    void Start() {
        IList<Goal> allGoals = GetComponentsInChildren<Goal>();
        GoalCount = allGoals.Count;
        GoalIndices = new int[GoalCount];
        for (int i = 0; i < allGoals.Count; i++) {
            GoalIndices[i] = allGoals[i].Index;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void SetGoalOrder(int[] newOrder) {
        for (int i = 0; i < GoalCount; i++) {
            Goal goal = allGoals[i];
            int newIdx = newOrder[i];

            Vector2 newGoalPos = new Vector2(firstX + newIdx * xOffset, goalY);
            goal.MoveToIndex(newOrder[i], newGoalPos);
        }
    }

    public void MultipleGoalScores(float multiplier) {
        foreach(Goal goal in allGoals) {
            goal.ChangeScore(goal.Scrore * multiplier);
        }
    }

    public void ResetGoalScores() {
        foreach (Goal goal in allGoals) {
            goal.ResetScore();
        }
    }
}
