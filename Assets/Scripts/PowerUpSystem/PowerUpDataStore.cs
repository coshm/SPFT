using UnityEngine;
using System;
using System.Collections.Generic;
using SPFT.PowerUpSystem.PowerUps;

namespace SPFT.PowerUpSystem {

    public class PowerUpDataStore : SingletonBase<PowerUpDataStore> {

        private DataLoader dataLoader;
        private ResourceLoader resourceLoader;

        private IDictionary<Guid, GameObject> powerUpsByGuid;
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
                GameObject powerUpPrefab = new GameObject();
                IPowerUp powerUp = powerUpPrefab.AddComponent(kvp.Key) as IPowerUp;
                powerUp.Initialize(kvp.Value.initArgs);

                powerUpsByGuid[powerUp.Id] = powerUpPrefab;
                powerUpIconsByGuid[powerUp.Id] = ResourceLoader.GetSpriteForPowerUp(powerUp.GetType());
            }
        }

        public IList<Guid> GetAllPowerUpGuids() {
            return powerUpsByGuid.Keys as IList<Guid>;
        }

        public GameObject GetPowerUpByGuid(Guid powerUpGuid) {
            return powerUpsByGuid[powerUpGuid];
        }

        public Sprite GetPowerUpIconByGuid(Guid powerUpGuid) {
            return powerUpIconsByGuid[powerUpGuid];
        }
    }
}
