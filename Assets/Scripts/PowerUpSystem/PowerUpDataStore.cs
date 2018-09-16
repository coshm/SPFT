using UnityEngine;
using System;
using System.Collections;
using SPFT.PowerUpSystem.PowerUps;

namespace SPFT.PowerUpSystem {

    public class PowerUpDataStore : SingletonBase<PowerUpDataStore> {

        private DataLoader dataLoader;
        private ResourceLoader resourceLoader;

        private IDictionary<Guid, IPowerUp> powerUpsByGuid;
        private IDictionary<Guid, Sprite> powerUpIconsByGuid;

        void Awake() {
            dataLoader = DataLoader.Instance;
            resourceLoader = ResourceLoader.Instance;
        }

        void Start() {
            powerUpsByGuid = new Dictionary<Guid, GameObject>();
            powerUpIconsByGuid = new Dictionary<Guid, Sprite>();
            IDictionary<Type, PowerUpData> powerUpDataByType = dataLoader.LoadPowerUpData();
            foreach (KeyValuePair<Type, PowerUpData> kvp in powerUpDataByType) {
                // Instatiate PowerUp and initialize it with args loaded from Json
                IPowerUp powerUp = new GameObject().AddComponent(kvp.Key) as IPowerUp;
                powerUp.Initialize(kvp.Value.initArgs);

                powerUpsByGuid[powerUp.Guid] = powerUp;
                powerUpIconsByGuid[powerUp.Guid] = resourceLoader.GetSpriteForPowerUp(powerUp.GetType());
            }
        }

        public IList<Guid> GetAllPowerUpGuids() {
            return powerUpsByGuid.Keys();
        }

        public IPowerUp GetPowerUpByGuid(Guid powerUpGuid) {
            return powerUpsByGuid[powerUpGuid];
        }

        public Sprite GetPowerUpIconByGuid(Guid powerUpGuid) {
            return powerUpIconsByGuid[powerUpGuid];
        }
    }
}
