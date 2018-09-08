namespace SPFT.EventSystem.Events {

    public enum CauseOfGameOver {
        BANKRUPT,
    }

    public struct GameOverEvent : IEvent {
        public CauseOfGameOver causeOfGameOver;
    }

}
