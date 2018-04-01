using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

    private const int DEFAULT_SCORE = 25;

    // public so it can be set in editor
    public int score;

    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject floor;

    private ScoreEvent scoreEvent;
    private PuckResetEvent puckResetEvent;

    void Awake() {
        if (score == 0) {
            score = DEFAULT_SCORE;
        }

        if (leftWall == null || rightWall == null || floor == null) {
            throw new InvalidOperationException("LeftWall, RightWall, and Floor must be set in the editor.");
        }

        scoreEvent = EventManager.Instance.GetOrAddEventWithPayload(new ScoreEvent());
        puckResetEvent = EventManager.Instance.GetOrAddEventWithPayload(new puckResetEvent());
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
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
