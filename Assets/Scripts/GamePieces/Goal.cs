using UnityEngine;
using System;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;

public class Goal : MonoBehaviour {
    
    [SerializeField]
    private int _score;
    public int Score {
        get { return _score; }
        set { _score = value; }
    }

    [SerializeField]
    private int _index;
    public int Index {
        get { return _index; }
        set { _index = value; }
    }

    private int? originalScore;

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
        goalText.text = Score.ToString();
    }
    
    void Start() {

    }
    
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "Puck") {
            // Puck has scored in this goal
            PuckScoreEvent puckScoreEvent = new PuckScoreEvent() {
                cashPrize = Score
            };
            EventManager.Instance.NotifyListeners(puckScoreEvent);
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
