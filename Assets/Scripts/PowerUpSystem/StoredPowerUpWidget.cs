using UnityEngine;
using System.Collections.Generic;

namespace SPFT.PowerUpSystem {

    public class StoredPowerUpWidget : MonoBehaviour {

        private Queue<SpriteRenderer> widget;

        private IDictionary<int, Sprite> emptySpotsByIndex;

        private PowerUpDataStore powerUpDataStore;
        
        void Awake() {
            powerUpDataStore = PowerUpDataStore.Instance;
        }
        
        void Update() {

        }

        public void ActivatePowerUp() {
            // Remove first powerup and shift everything else forward
        }
    }
}
