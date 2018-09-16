using UnityEngine;
using System;

namespace SPFT.PowerUpSystem.PowerUps {

    public class RearrangeStoredPowerUps : PowerUpBase {

        private const int INIT_ARG_COUNT = 2;

        private PowerUpLifeCycleManager pwrUpLifeCycleMgr;
        private StoredPowerUpWidget storedPowerUpWidget;
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
            pwrUpLifeCycleMgr = PowerUpLifeCycleManager.Instance;
            storedPowerUpWidget = StoredPowerUpWidget.Instance;
        }

        // Update is called once per frame
        void Update() {
            if (IsActive && Input.GetMouseButtonDown(0)) {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && storedPowerUpWidget.IsAPowerUpIcon(hit.collider.gameObject)) {
                    int iconIdx = storedPowerUpWidget.GetIndexFromIconTag(hit.collider.gameObject);
                    reorderedIconIndices[reorderedIconIndices.Length] = iconIdx;

                    if (reorderedIconIndices.Length == pwrUpLifeCycleMgr.StoredPowerUpCount) {
                        pwrUpLifeCycleMgr.SetStoredPowerUpOrder(reorderedIconIndices);
                        Deactivate();
                    }
                }
            }
        }

        public override void Activate() {
            Debug.Log($"Activating {GetType()} PowerUp.");
            IsActive = true;
            if (pwrUpLifeCycleMgr.StoredPowerUpCount == 0) {
                Deactivate();
            } else {
                reorderedIconIndices = new int[pwrUpLifeCycleMgr.StoredPowerUpCount];
            }
        }

        public override void Deactivate() {
            Debug.Log($"Deactivating {GetType()} PowerUp.");
            IsActive = false;
            EmitExpiredEventAndSelfDestruct(this, gameSettings.pwrUpPostDeactivationDelay);
        }
    }
}