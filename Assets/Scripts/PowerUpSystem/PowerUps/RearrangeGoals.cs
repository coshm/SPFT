using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace SPFT.PowerUpSystem.PowerUps {

    public class RearrangeGoals : PowerUpBase {

        private GoalManager goalMgr;
        private int[] originalOrder;

        public override void Initialize(params PowerUpArg[] args) {
            InitializeBase(args);
        }

        void Awake() {
            goalMgr = GoalManager.Instance;
        }

        public override void Activate() {
            IsActive = true;
            originalOrder = goalMgr.GoalIndices;
            goalMgr.SetGoalOrder(RandomizeOrder(goalMgr.GoalCount));
        }
        
        public override void Deactivate() {
            IsActive = false;
            goalMgr.SetGoalOrder(originalOrder);
            Destroy(this);
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
