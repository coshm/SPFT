using UnityEngine;
using System;
using System.Collections;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;

public class PowerUpTimer : MonoBehaviour {

    private Text timerText;

    public Guid CurrentTimerId { get; private set; }
    public bool IsCountingDown { get; private set; }
    public float CurrentTime { get; private set; }

    void Awake() {
        timerText = GetComponent<Text>();
        CurrentTimerId = null;
    }

    void Update() {
        if (CurrentTimerId != null) {
            int minutes = Mathf.FloorToInt(CurrentTime.Value / 60F);
            int seconds = Mathf.FloorToInt(CurrentTime.Value % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (IsCountingDown) {
                CurrentTime -= Time.deltaTime;
                if (CurrentTime <= 0f) {
                    // Notify all the listeners that the PowerUp has been Deactivated
                    PowerUpTimerExpiredEvent powerUpTimerExpiredEvent = new PowerUpTimerExpiredEvent() {
                        timerId = CurrentTimerId
                    };
                    EventManager.Instance.NotifyListeners(powerUpExpiredEvent);
                }
            } else {
                CurrentTime += Time.deltaTime;
            }
        }
    }

    public Guid StartTimer(float seconds) {
        if (CurrentTimerId != null) {
            throw new InvalidOperationException($"Timer with Id={CurrentTimerId}, is already " +
                                                $"running with remaining time of {CurrentTime}");
        }
        CurrentTime = seconds;
        IsCountingDown = true;

        // Create a new Id for this timer so the requester knows when it has ended.
        CurrentTimerId = Guid.NewGuid();
        return CurrentTimerId;
    }

    public float ClearTimer() {
        CurrentTimerId = null;
    }

    public Guid StartStopWatch() {
        if (CurrentTimerId != null) {
            throw new InvalidOperationException($"Timer with Id={CurrentTimerId}, is already " +
                                                $"running with remaining time of {CurrentTime}");
        }
        CurrentTime = 0f;
        IsCountingDown = false;

        // Create a new Id for this timer so the requester knows when it has ended.
        CurrentTimerId = Guid.NewGuid();
        return CurrentTimerId;
    }
}
