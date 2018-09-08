using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SPFT.PowerUpSystem.PowerUps {

    public class RearrangeGoals : MonoBehaviour, IPowerUp {

        private const int INIT_ARG_COUNT = 2;

        private const string ID = "id";
        private const string ICON = "icon";

        private GoalManager goalMgr;
        private int[] originalOrder;

        public Guid Id { get; private set; }
        public Sprite Icon { get; private set; }
        public bool IsActive { get; private set; }

        public void Initialize(params PowerUpArg[] args) {
            if (args.Length != INIT_ARG_COUNT) {
                throw new InvalidOperationException($"Expected {INIT_ARG_COUNT} init args but actually got {args.Length}.");
            }

            IsActive = false;
            foreach (PowerUpArg arg in args) {
                switch (arg.name) {
                    case ID:
                        Id = Guid.Parse(arg.value);
                        break;
                    case ICON:
                        Icon = Resources.Load<Sprite>(arg.value);
                        break;
                    default:
                        throw new InvalidOperationException($"No parameter found for {arg.name}");
                }
            }
        }
        
        // Use this for initialization
        void Awake() {
            goalMgr = GoalManager.Instance;
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

        public bool IsBlocked(List<IPowerUp> activePowerUps) {
            foreach (IPowerUp powerUp in activePowerUps) {
                if (powerUp.GetType() == this.GetType()) {
                    return true;
                }
            }
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
}
