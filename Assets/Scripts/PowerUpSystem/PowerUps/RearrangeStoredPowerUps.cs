using UnityEngine;

namespace SPFT.PowerUpSystem.PowerUps {

    public class RearrangeStoredPowerUps : PowerUpBase {

        private PowerUpManager pwrUpMgr;
        private int[] reorderedIconIndices;

        public override void Initialize(params PowerUpArg[] args) {
            InitializeBase(args);
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
            IsActive = true;
            if (pwrUpMgr.StoredPowerUpCount == 0) {
                Deactivate();
            } else {
                reorderedIconIndices = new int[pwrUpMgr.StoredPowerUpCount];
            }
        }

        public override void Deactivate() {
            IsActive = false;
            Destroy(this);
        }

    }
}