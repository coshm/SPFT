using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

    private const int DEFAULT_SCORE = 25;

    public int score;

    void Awake() {
        if (score == 0) {
            score = DEFAULT_SCORE;
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter2D (Collider2D coll) {
        if (coll.gameObject.tag == "Puck") {
            //ScoreManager.Instance.
        }
    }
}
