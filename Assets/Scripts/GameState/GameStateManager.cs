using System;
using System.Collections.Generic;
using UnityEngine;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;

namespace SPFT.State {

    public enum MainGameState {
        PRE_LAUNCH,
        LAUNCH_POSITIONING,
        LAUNCH_AIMING,
        PUCK_DROPPING,
        GAME_PAUSED
    }

    public class GameStateManager : MonoBehaviour {

        private static GameStateManager gameStateMgr;
        public static GameStateManager Instance
        {
            get
            {
                if (!gameStateMgr) {
                    gameStateMgr = FindObjectOfType(typeof(GameStateManager)) as GameStateManager;
                    if (!gameStateMgr) {
                        Debug.LogError("There needs to be one active GameStateManager script on a GameObject in your scene.");
                    } else {
                        gameStateMgr.Init();
                    }
                }
                return gameStateMgr;
            }
        }

        void Init() { }

        public MainGameState State { get; private set; }
        private MainGameState unpausedState;
        
        void Start() {
            State = MainGameState.PRE_LAUNCH;

            EventManager eventMgr = EventManager.Instance;
            eventMgr.RegisterListener<BuyPuckEvent>(OnBuyPuck);
            eventMgr.RegisterListener<PuckAimingEvent>(OnPuckAiming);
            eventMgr.RegisterListener<PuckLaunchEvent>(OnPuckLaunch);
            eventMgr.RegisterListener<KillPuckEvent>(OnPuckDeath);
            eventMgr.RegisterListener<PuckScoreEvent>(OnPuckScore);
            eventMgr.RegisterListener<PauseGameEvent>(OnGamePause);
            eventMgr.RegisterListener<UnpauseGameEvent>(OnGameUnpause);
            eventMgr.RegisterListener<GameOverEvent>(OnGameOver);
        }
        
        void Update() {

        }

        public void OnBuyPuck(BuyPuckEvent buyPuckEvent) {
            Debug.Log($"GameStateManager handling BuyPuckEvent. CurrentState={State}, Event={buyPuckEvent}");
            if (State != MainGameState.PRE_LAUNCH) {
                throw new InvalidOperationException("BuyPuckEvent should only occur during PRE_LAUNCH state");
            }
            State = MainGameState.LAUNCH_POSITIONING;
        }

        public void OnPuckAiming(PuckAimingEvent puckAimingEvent) {
            Debug.Log($"GameStateManager handling PuckAimingEvent. CurrentState={State}, Event={puckAimingEvent}");
            if (State != MainGameState.LAUNCH_POSITIONING) {
                throw new InvalidOperationException("PuckAimingEvent should only occur during LAUNCH_POSITIONING state");
            }
            State = MainGameState.LAUNCH_AIMING;
        }

        public void OnPuckLaunch(PuckLaunchEvent puckLaunchEvent) {
            Debug.Log($"GameStateManager handling PuckLaunchEvent. CurrentState={State}, Event={puckLaunchEvent}");
            if (State != MainGameState.LAUNCH_AIMING) {
                throw new InvalidOperationException("PuckLaunchEvent should only occur during LAUNCH_AIMING state");
            }
            State = MainGameState.PUCK_DROPPING;
        }

        public void OnPuckDeath(KillPuckEvent killPuckEvent) {
            Debug.Log($"GameStateManager handling KillPuckEvent. CurrentState={State}, Event={killPuckEvent}");
            if (State != MainGameState.PUCK_DROPPING) {
                throw new InvalidOperationException("KillPuckEvent should only occur during PUCK_DROPPING state");
            }
            State = MainGameState.PRE_LAUNCH;
        }

        public void OnPuckScore(PuckScoreEvent puckScoreEvent) {
            Debug.Log($"GameStateManager handling PuckScoreEvent. CurrentState={State}, Event={puckScoreEvent}");
            if (State != MainGameState.PUCK_DROPPING) {
                throw new InvalidOperationException("PuckScoreEvent should only occur during PUCK_DROPPING state");
            }
            State = MainGameState.PRE_LAUNCH;
        }

        public void OnGamePause(PauseGameEvent pauseGameEvent) {
            Debug.Log($"GameStateManager handling PauseGameEvent. CurrentState={State}, Event={pauseGameEvent}");
            if (State == MainGameState.GAME_PAUSED) {
                throw new InvalidOperationException("PauseGameEvent cannot occur during GAME_PAUSED state");
            }
            unpausedState = State;
            State = MainGameState.GAME_PAUSED;
        }

        public void OnGameUnpause(UnpauseGameEvent unpauseGameEvent) {
            Debug.Log($"GameStateManager handling UnpauseGameEvent. CurrentState={State}, Event={unpausedState}");
            if (State != MainGameState.GAME_PAUSED) {
                throw new InvalidOperationException("UnpauseGameEvent should only occur during GAME_PAUSED state");
            }
            State = unpausedState;
        }

        public void OnGameOver(GameOverEvent gameOverEvent) {
            Debug.Log($"GameStateManager handling GameOverEvent. CurrentState={State}, Event={gameOverEvent}");
            Application.Quit();
        }
    }
}

