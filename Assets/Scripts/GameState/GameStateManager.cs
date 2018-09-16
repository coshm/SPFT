using System;
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

    public enum SlotMachineState {
        AT_REST,
        START_SPINNING,
        SPINNING,
        STOP_SPINNING
    }

    public class GameStateManager : SingletonBase<GameStateManager> {
        
        public MainGameState State { get; private set; }
        private MainGameState unpausedState;

        public SlotMachineState SlotState { get; private set; }
        private const string SLOT_STATE_ERROR_MSG = "Invalid State. Attempted to transition from SlotState {0} to {1}. Valid transitions are {2}.";
        
        void Start() {
            State = MainGameState.PRE_LAUNCH;
            SlotState = SlotMachineState.AT_REST;

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

        /* ~~~~~~~~~~~~~~~~~~~~~~~~ Main Game State Handlers ~~~~~~~~~~~~~~~~~~~~~~~~ */

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

        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~ Slot State Handlers ~~~~~~~~~~~~~~~~~~~~~~~~~~ */

        public void SetSlotMachineState(SlotMachineState slotState) {
            switch (SlotState) {
                case SlotMachineState.AT_REST:
                    if (slotState != SlotMachineState.START_SPINNING) {
                        throw new ArgumentException(string.Format(SLOT_STATE_ERROR_MSG, SlotState, slotState, SlotMachineState.START_SPINNING));
                    }
                    break;
                case SlotMachineState.START_SPINNING:
                    if (slotState != SlotMachineState.SPINNING) {
                        throw new ArgumentException(string.Format(SLOT_STATE_ERROR_MSG, SlotState, slotState, SlotMachineState.SPINNING));
                    }
                    break;
                case SlotMachineState.SPINNING:
                    if (slotState != SlotMachineState.STOP_SPINNING) {
                        throw new ArgumentException(string.Format(SLOT_STATE_ERROR_MSG, SlotState, slotState, SlotMachineState.STOP_SPINNING));
                    }
                    break;
                case SlotMachineState.STOP_SPINNING:
                    if (slotState != SlotMachineState.AT_REST) {
                        throw new ArgumentException(string.Format(SLOT_STATE_ERROR_MSG, SlotState, slotState, SlotMachineState.AT_REST));
                    }
                    break;
            }

            SlotState = slotState;
        }
    }
}

