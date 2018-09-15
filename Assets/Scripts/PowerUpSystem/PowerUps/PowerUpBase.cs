using UnityEngine;
using System;
using System.Collections.Generic;

namespace SPFT.PowerUpSystem.PowerUps {

    public abstract class PowerUpBase : MonoBehaviour, IPowerUp {

        // Initialization Arg Names
        protected const string ID = "id";
        protected const string ICON = "icon";
        
        public Guid Id { get; protected set; }
        public Sprite Icon { get; protected set; }
        public bool IsActive { get; protected set; }

        protected GameSettings gameSettings;

        public void InitializeBase(params PowerUpArg[] args) {
            gameSettings = GameSettings.Instance;
            IsActive = false;
            foreach (PowerUpArg arg in args) {
                switch (arg.name) {
                    case ID:
                        Id = Guid.Parse(arg.value);
                        break;
                    case ICON:
                        string[] spriteArgs = arg.value.Split(',');
                        Sprite[] powerUpIcons = Resources.LoadAll<Sprite>(spriteArgs[0]);
                        Icon = powerUpIcons[int.Parse(spriteArgs[1])];
                        break;
                    default:
                        break;
                }

                if (Id != null && Icon != null) {
                    return;
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

        protected void EmitExpiredEventAndSelfDestruct(IPowerUp powerUp, float delay) {
            PowerUpExpiredEvent powerUpExpiredEvent = new PowerUpExpiredEvent() {
                powerUp = powerUp
            };
            EventManager.Instance.NotifyListeners(powerUpExpiredEvent);

            StartCoroutine(SelfDestructAfterDelay(delay));
        }

        protected IEnumerator SelfDestructAfterDelay(float delay) {
            yield return new WaitForSeconds(delay);
            Destroy(this);
        }
    }
}
