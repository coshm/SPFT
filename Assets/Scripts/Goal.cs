using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;

public class Goal : MonoBehaviour {

    // public so it can be set in editor
    public int score;

    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject floor;

    private TextMesh goalText;

    void Awake() {
        if (leftWall == null || rightWall == null || floor == null) {
            throw new InvalidOperationException("LeftWall, RightWall, and Floor must be set in the editor.");
        }

        goalText = GetComponentInChildren<TextMesh>();
        if (goalText == null) {
            throw new InvalidOperationException("Goal's child object must have a TextMesh component.");
        }
        goalText.text = score.ToString();
    }
    
    void Start() {

    }
    
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "Puck") {
            // Puck has scored in this goal
            PuckScoreEvent puckScoreEvent = new PuckScoreEvent() {
                cashPrize = score
            };
            EventManager.Instance.NotifyListeners(puckScoreEvent);
        }
    }
    
    public void ChangeScore(int newScore) {
        score = newScore;
        goalText.text = newScore.ToString();
    }

}
