namespace SPFT.EventSystem.Events {

    public enum CauseOfDeath {
        OUT_OF_BOUNDS,
    }

    public struct KillPuckEvent : IEvent {
        public CauseOfDeath causeOfDeath;
        public int cashPenalty;
    }

}
