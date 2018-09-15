using UnityEngine;
using System;

namespace SPFT.PowerUpSystem.PowerUps {

    public class RearrangeStoredPowerUps : PowerUpBase {

        private const int INIT_ARG_COUNT = 2;

        private PowerUpManager pwrUpMgr;
        private int[] reorderedIconIndices;

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
        
        // Use this for initialization
        void Awake() {
            pwrUpMgr = PowerUpManager.Instance;
        }

        // Update is called once per frame
        void Update() {
            if (IsActive && Input.GetMouseButtonDown(0)) {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && pwrUpMgr.IsAPowerUpIcon(hit.collider.gameObject)) {
                    int iconIdx = pwrUpMgr.GetIndexFromIconTag(hit.collider.gameObject);
                    reorderedIconIndices[reorderedIconIndices.Length] = iconIdx;

                    if (reorderedIconIndices.Length == pwrUpMgr.StoredPowerUpCount) {
                        pwrUpMgr.SetStoredPowerUpOrder(reorderedIconIndices);
                        Deactivate();
                    }
                }
            }
        }

        public override void Activate() {
            Debug.Log($"Activating {GetType()} PowerUp.");
            IsActive = true;
            if (pwrUpMgr.StoredPowerUpCount == 0) {
                Deactivate();
            } else {
                reorderedIconIndices = new int[pwrUpMgr.StoredPowerUpCount];
            }
        }

        public override void Deactivate() {
            Debug.Log($"Deactivating {GetType()} PowerUp.");
            IsActive = false;
            EmitExpiredEventAndSelfDestruct(this, gameSettings.pwrUpPostDeactivationDelay);
        }
    }
}