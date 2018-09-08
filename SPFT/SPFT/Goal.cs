using UnityEngine;
using System;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;

public class Goal : MonoBehaviour {

    // public so it can be set in editor
    public int Score { get; private set; }
    public int Index { get; private set; }

    private int? originalScore;

    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject floor;

    void Awake() {
        if (leftWall == null || rightWall == null || floor == null) {
            throw new InvalidOperationException("LeftWall, RightWall, and Floor must be set in the editor.");
        }
    }
    
    void Start() { }
    
    void Update() { }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "Puck") {
            // Puck has scored in this goal
            ScoreEvent scoreEvent = new ScoreEvent()
            {
                score = Score,
            };
            EventManager.Instance.NotifyListeners<ScoreEvent>(scoreEvent);

            // Reset puck back to starting position
            PuckResetEvent puckResetEvent = new PuckResetEvent();
            EventManager.Instance.NotifyListeners<PuckResetEvent>(puckResetEvent);
        }
    }

    public void ChangeScore(int newScore) {
        originalScore = Score;
        Score = newScore;
    }

    public void ResetScore() {
        if (originalScore != null) {
            Score = originalScore.Value;
            originalScore = null;
        }
    }

    public void MoveToIndex(int newIndex, Vector2 newPos) {
        Index = newIndex;
        transform.localPosition = newPos;
    }

}
