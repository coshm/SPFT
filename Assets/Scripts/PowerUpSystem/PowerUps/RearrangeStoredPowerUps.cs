using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SPFT.PowerUpSystem.PowerUps {

    public class RearrangeStoredPowerUps : MonoBehaviour, IPowerUp {

        private const int INIT_ARG_COUNT = 4;

        private const string ID = "id";
        private const string ICON = "icon";

        public Guid Id { get; private set; }
        public Sprite Icon { get; private set; }
        public bool IsActive { get; private set; }

        private PowerUpManager pwrUpMgr;
        private int[] reorderedIconIndices;

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

        public void Activate() {
            IsActive = true;
            if (pwrUpMgr.StoredPowerUpCount == 0) {
                Deactivate();
            } else {
                reorderedIconIndices = new int[pwrUpMgr.StoredPowerUpCount];
            }
        }

        public void Deactivate() {
            IsActive = false;
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
    }
}