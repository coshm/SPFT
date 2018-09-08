using System;
using System.Collections.Generic;
using UnityEngine;

namespace SPFT.PowerUpSystem.PowerUps {

    public class PegExploder : MonoBehaviour, IPowerUp {

        private const int INIT_ARG_COUNT = 5;

        // Initialization Arg Names
        #region
        private const string ID = "id";
        private const string ICON = "icon";
        private const string PEG_RESPAWN_DELAY = "pegRespawnDelay";
        private const string MAX_EXPLOSION_COUNT = "maxExplosionCount";
        private const string EXPLOSION_EFFECT = "explosionEffect";
        #endregion

        public Guid Id { get; private set; }
        public Sprite Icon { get; private set; }
        public bool IsActive { get; private set; }
        public float pegRespawnDelay;
        public int maxExplosionCount;
        public AnimationClip explosionEffect; // Change to some Animation object?

        private GameSettings gameSettings;

        private int pegsExplodedCount = 0;

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
                    case PEG_RESPAWN_DELAY:
                        pegRespawnDelay = Utilities.ConvertStringOrDefault(arg.value, gameSettings.pegRespawnDelay);
                        break;
                    case MAX_EXPLOSION_COUNT:
                        maxExplosionCount = Utilities.ConvertStringOrDefault(arg.value, gameSettings.maxExplosionCount);
                        break;
                    case EXPLOSION_EFFECT:
                        explosionEffect = Resources.Load<AnimationClip>(arg.value); // TODO: figure out how to load/play animation clips
                        break;
                    default:
                        throw new InvalidOperationException($"No parameter found for {arg.name}");
                }
            }
        }

        void Awake() {
            gameSettings = GameSettings.Instance;
        }
        
        void Update() {
            if (IsActive && Input.GetMouseButtonDown(0)) {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.CompareTag(GameSettings.Instance.pegTag)) {
                    Peg peg = hit.collider.GetComponent<Peg>();
                    Vector2 pegPosition = peg.transform.position;
                    peg.Destroy(pegRespawnDelay);

                    // handle explosion effect

                    pegsExplodedCount++;
                    if (pegsExplodedCount >= maxExplosionCount) {
                        Deactivate();
                    }
                }
            }
        }

        public void Activate() {
            IsActive = true;
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