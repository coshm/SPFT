using System;
using UnityEngine;

namespace SPFT.PowerUpSystem.PowerUps {

    public class PegExploder : PowerUpBase {

        private const int INIT_ARG_COUNT = 5;

        // Initialization Arg Names
        #region
        private const string PEG_RESPAWN_DELAY = "pegRespawnDelay";
        private const string MAX_EXPLOSION_COUNT = "maxExplosionCount";
        private const string EXPLOSION_EFFECT = "explosionEffect";
        #endregion

        public float pegRespawnDelay;
        public int maxExplosionCount;
        public AnimationClip explosionEffect; // Change to some Animation object?

        private int pegsExplodedCount = 0;

        public override void Initialize(params PowerUpArg[] args) {
            InitializeBase(args);
            if (args.Length != INIT_ARG_COUNT) {
                throw new InvalidOperationException($"Expected {INIT_ARG_COUNT} init args but actually got {args.Length}.");
            }
            
            foreach (PowerUpArg arg in args) {
                switch (arg.name) {
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

        public override void Activate() {
            IsActive = true;
        }

        public override void Deactivate() {
            IsActive = false;
            Destroy(this);
        }
        
    }
}