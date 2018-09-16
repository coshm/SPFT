using UnityEngine;
using UnityEngine.UI;
using System;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;

namespace SPFT.PowerUpSystem {

    public class PowerUpTimer : SingletonBase<PowerUpTimer> {

        private Text timerText;

        public Guid? CurrentTimerId { get; private set; }
        public bool IsCountingDown { get; private set; }
        public float CurrentTime { get; private set; }

        void Awake() {
            timerText = GetComponent<Text>();
            CurrentTimerId = null;
            CurrentTime = 0f;
        }

        void Update() {
            if (CurrentTimerId.HasValue) {
                int minutes = Mathf.FloorToInt(CurrentTime / 60F);
                int seconds = Mathf.FloorToInt(CurrentTime % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                if (IsCountingDown) {
                    CurrentTime -= Time.deltaTime;
                    if (CurrentTime <= 0f) {
                        // Notify all the listeners that the PowerUp has been Deactivated
                        PowerUpTimerExpiredEvent powerUpTimerExpiredEvent = new PowerUpTimerExpiredEvent() {
                            timerId = CurrentTimerId.Value
                        };
                        EventManager.Instance.NotifyListeners(powerUpTimerExpiredEvent);
                    }
                } else {
                    CurrentTime += Time.deltaTime;
                }
            }
        }

        public Guid StartTimer(float seconds) {
            if (CurrentTimerId.HasValue) {
                throw new InvalidOperationException($"Timer with Id={CurrentTimerId.Value}, is already " +
                                                    $"running with remaining time of {CurrentTime}");
            }
            CurrentTime = seconds;
            IsCountingDown = true;

            // Create a new Id for this timer so the requester knows when it has ended.
            CurrentTimerId = Guid.NewGuid();
            return CurrentTimerId.Value;
        }

        public void ClearTimer() {
            CurrentTimerId = null;
        }

        public Guid StartStopWatch() {
            if (CurrentTimerId.HasValue) {
                throw new InvalidOperationException($"Timer with Id={CurrentTimerId.Value}, is already " +
                                                    $"running with remaining time of {CurrentTime}");
            }
            CurrentTime = 0f;
            IsCountingDown = false;

            // Create a new Id for this timer so the requester knows when it has ended.
            CurrentTimerId = Guid.NewGuid();
            return CurrentTimerId.Value;
        }
    }
}
