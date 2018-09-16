using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using SPFT.PowerUpSystem.PowerUps;

namespace SPFT.PowerUpSystem {

    public class StoredPowerUpWidget : SingletonBase<StoredPowerUpWidget> {

        public const string ICON_TAG_PREFIX = "StoredPowerUpIcon";
        public const char ICON_TAG_DELIMETER = '_';

        [SerializeField]
        public IDictionary<int, SpriteRenderer> storedPowerUpIconSlots;
        public Sprite emptyPowerUpIconSlot;

        private PowerUpDataStore powerUpDataStore;
        private GameSettings gameSettings;

        void Awake() {
            gameSettings = GameSettings.Instance;
            powerUpDataStore = PowerUpDataStore.Instance;

            // Get the SpriteRenderers from all child GOs, which are all the icon slots for store powerUps.
            storedPowerUpIconSlots = new Dictionary<int, SpriteRenderer>();
            IList<SpriteRenderer> iconSlots = GetComponentsInChildren<SpriteRenderer>();
            if (iconSlots == null || iconSlots.Count != gameSettings.maxStoredPowerUps) {
                throw new InvalidOperationException($"Only found {iconSlots.Count} icon slots, not the expected {gameSettings.maxStoredPowerUps}");
            }

            // Initialize each iconSlot with by setting the empty iconSlot sprite and getting their indices.
            emptyPowerUpIconSlot = ResourceLoader.GetSprite(ResourceLoader.POWER_UP_ICONS, 6);
            foreach (SpriteRenderer iconSlot in iconSlots) {
                int idx = GetIndexFromIconTag(iconSlot.gameObject);
                iconSlot.sprite = emptyPowerUpIconSlot;
                storedPowerUpIconSlots[idx] = iconSlot;
            }
        }

        public bool IsAPowerUpIcon(GameObject obj) {
            return obj.tag.StartsWith(ICON_TAG_PREFIX);
        }

        public int GetIndexFromIconTag(GameObject iconObj) {
            Debug.Log($"Getting Index from IconTag={iconObj.tag}");
            return int.Parse(iconObj.tag.Split(ICON_TAG_DELIMETER)[1]);
        }

        public void UpdateStoredPowerUpIcons(Queue<IPowerUp> storedPowerUps) {
            for (int i = 0; i < storedPowerUpIconSlots.Count; i++) {
                storedPowerUpIconSlots[i].sprite = storedPowerUps.ElementAt(i)?.Icon;
            }
        }
    }
}
