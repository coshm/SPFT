using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SPFT.PowerUpSystem.PowerUps {

    public class RearrangeGoals : PowerUpBase {

        private const int INIT_ARG_COUNT = 2;

        private GoalManager goalMgr;
        private int[] originalOrder;

        public override void Initialize(params PowerUpArg[] args) {
            Debug.Log($"Initializing {GetType()} with args={args}");

            if (args.Length != INIT_ARG_COUNT) {
                throw new InvalidOperationException($"Expected {INIT_ARG_COUNT} init args but actually got {args.Length}.");
            }

            foreach (PowerUpArg arg in args) {
                switch (arg.name) {
                    case ID:
                    case ICON:
                        InitializeBase(arg);
                        break;
                    default:
                        throw new InvalidOperationException($"No parameter found for {arg.name}");
                }
            }
        }

        void Awake() {
            goalMgr = GoalManager.Instance;
        }

        public override void Activate() {
            Debug.Log($"Activating {GetType()} PowerUp.");
            IsActive = true;
            originalOrder = goalMgr.GoalIndices;
            goalMgr.SetGoalOrder(RandomizeOrder(goalMgr.GoalCount));
        }
        
        public override void Deactivate() {
            Debug.Log($"Deactivating {GetType()} PowerUp.");
            IsActive = false;
            goalMgr.SetGoalOrder(originalOrder);
            EmitExpiredEventAndSelfDestruct(this, gameSettings.pwrUpPostDeactivationDelay);
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
