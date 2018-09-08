using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GoalManager : MonoBehaviour
{
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
    void Start()
    {
        IList<Goal> allGoals = GetComponentsInChildren<Goal>();
        GoalCount = allGoals.Count;
        GoalIndices = new int[GoalCount];
        for (int i = 0; i < allGoals.Count; i++) {
            GoalIndices[i] = allGoals[i].Index;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetGoalOrder(int[] newOrder) {
        for (int i = 0; i < GoalCount; i++) {
            Goal goal = allGoals[i];
            int newIdx = newOrder[i];

            Vector2 newGoalPos = newGoalPos Vector2(firstX + newIdx * xOffset, goalY);
            goal.MoveToIndex(newOrder[i], newGoalPos);
        }
    }
}
