using UnityEngine;
using System;

public class Goal : MonoBehaviour {

    // public so it can be set in editor
    public int score;

    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject floor;

    private ScoreEvent scoreEvent;
    private PuckResetEvent puckResetEvent;

    void Awake() {
        if (leftWall == null || rightWall == null || floor == null) {
            throw new InvalidOperationException("LeftWall, RightWall, and Floor must be set in the editor.");
        }
    }
    
    void Start() {
        scoreEvent = EventManager.Instance.GetOrAddEventWithPayload(new ScoreEvent());
        puckResetEvent = EventManager.Instance.GetOrAddEventWithPayload(new PuckResetEvent());
    }
    
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "Puck") {
            // Puck has scored in this goal
            ScorePayload scorePayload = new ScorePayload(score);
            scoreEvent.Invoke(scorePayload);

            // Reset puck back to starting position
            PuckResetPayload puckResetPayload = new PuckResetPayload();
            puckResetEvent.Invoke(puckResetPayload);
            // If there is no payload, then it would just be
            // puckResetEvent.Invoke(puckResetPayload);
        }
    }

    public void ChangeScore(int newScore) {
        score = newScore;
    }

}
