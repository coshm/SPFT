using UnityEngine;
using System;
using System.Collections.Generic;

namespace SPFT.PowerUpSystem.PowerUps {

    public abstract class PowerUpBase : MonoBehaviour, IPowerUp {

        private const int BASE_INIT_ARG_COUNT = 2;

        // Initialization Arg Names
        private const string ID = "id";
        private const string ICON = "icon";
        
        public Guid Id { get; protected set; }
        public Sprite Icon { get; protected set; }
        public bool IsActive { get; protected set; }

        protected GameSettings gameSettings;

        void Awake() {
            gameSettings = GameSettings.Instance;
        }

        public void InitializeBase(params PowerUpArg[] args) {
            if (args.Length < BASE_INIT_ARG_COUNT) {
                throw new InvalidOperationException($"Expected at least {BASE_INIT_ARG_COUNT} init args but actually got {args.Length}.");
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
                        break;
                }

                if (Id == null || Icon == null) {
                    throw new InvalidOperationException("Failed to initialize ID and/or ICON.");
                }
            }
        }

        public abstract void Initialize(params PowerUpArg[] args);

        public abstract void Activate();

        public abstract void Deactivate();

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
