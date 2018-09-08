using UnityEngine;
using System;
using System.Type;
using System.Collections;
using System.Collections.Generic;

public class RearrangeGoals : MonoBehaviour, IPowerUp
{
    private const int INITIALIZE_ARGS = 2;

    private GoalManager goalMgr;
    private int[] originalOrder;

    public Guid Id { get; private set; }
    public Sprite Icon { get; private set; } 
    public bool IsActive { get; private set; }

    // Use this for initialization
    void Start()
    {

    }

    private void Initialize(params string[] args) {
        if (args == null || args.Length != INITIALIZE_ARGS) {
            throw new InvalidOperationException("Expected ${INITIALIZE_ARGS} args, got args=${args}");
        }

        Id = Guid.Parse(args[0]);
        Icon = Resources.Load<Sprite>(args[1]);
        IsActive = false;
    }

    public void Activate() {
        IsActive = true;
        originalOrder = goalMgr.GoalIndices;
        goalMgr.SetGoalOrder(RandomizeOrder(goalMgr.GoalCount));
    }

    public void Deactivate() {
        IsActive = false;
        goalMgr.SetGoalOrder(originalOrder);
        Destroy(this);
    }

    public bool IsBlockingPowerUpActivation(IPowerUp pwrUp) {
        return pwrUp.GetType() == typepf(this);
    }

    public bool OnPowerUpTrigger(IPowerUpTrigger pwrUpTrigger) {
        return false;
    }

    private int[] RandomizeOrder(int count) {
        IList<int> orderedIndices = new List<int>();
        for (int i = 0; i < count; i++) {
            orderedIndices.Add(i);
        }

        int[] randomizedIndices = new int[count];
        for (int i = 0; i < count; i++) {
            int randomIdx = Random.Range(0, orderedIndices.Count);
            randomizedIndices[i] = orderedIndices[randomIdx];
            orderedIndices.RemoveAt(randomIdx);
        }

        return randomizedIndices;
    }
}
