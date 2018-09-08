using System;

public class ScorePayload2 : IEventPayload {

    public int Score { get; private set; }

    public ScorePayload(int score) {
        Score = score;
    }

    public Type GetPayloadType() {
        return GetType();
    }

}
