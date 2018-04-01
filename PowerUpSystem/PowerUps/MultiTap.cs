/*using Color = CircleColor.Color;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MultiTap : MonoBehaviour, IPowerUp
{

    private const int DEFAULT_MAX_TAP_COUNT = 2;
    private const float DEFAULT_DURATION = 30f;

    public int maxTapCount;
    public float duration;

    public Color playerColor;
    public PowerUpAcquiredEvent pwrUpAcquiredEvent;
    public PowerUpExpiredEvent pwrUpExpiredEvent;
    public Dictionary<Guid, int> circleTapCounts;

    void Awake()
    {
        if (pwrUpAcquiredEvent == null)
        {
            pwrUpAcquiredEvent = new PowerUpAcquiredEvent();
        }
        if (pwrUpExpiredEvent == null)
        {
            pwrUpExpiredEvent = new PowerUpExpiredEvent();
        }
        EventManager.Instance.AddEvent(pwrUpAcquiredEvent);
        EventManager.Instance.AddEvent(pwrUpExpiredEvent);

        if (maxTapCount == 0)
        {
            maxTapCount = DEFAULT_MAX_TAP_COUNT;
        }
        if (duration == 0f)
        {
            duration = DEFAULT_DURATION;
        }
    }

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration < 0)
        {
            PowerUpExpiredPayload pwrUpExpiredPayload = new PowerUpExpiredPayload(this);
            pwrUpExpiredEvent.Invoke(pwrUpExpiredPayload);
            Destroy(gameObject);
        }
    }

    public void Activate()
    {
        Debug.Log("Activating MultiTap");
        circleTapCounts = new Dictionary<Guid, int>();
    }

    public bool IsBlockingPowerUpActivation(IPowerUp pwrUp)
    {
        return false;
    }

    // Handle any PowerUpTriggers relevant to the MultiTap PowerUp
    public bool OnPowerUpTrigger(IPowerUpTrigger pwrUpTrigger)
    {
        Debug.Log("MultiTap.OnPowerUpTrigger " + pwrUpTrigger.GetType());
        if (pwrUpTrigger.GetType() == typeof(ColorChangeTrigger))
        {
            Debug.Log("PwrUp is ColorChangeTrigger");
            ColorChangeTrigger colorChangeTrigger = (ColorChangeTrigger)pwrUpTrigger;
            CircleColor sourceCircle = colorChangeTrigger.SourceCircle;
            CircleColor targetCircle = colorChangeTrigger.TargetCircle;

            Debug.Log($"Trying to change circle {targetCircle.Id}'s color from {targetCircle.ActiveColor} to {colorChangeTrigger.NewColor} when player's color is {playerColor}.");

            if (targetCircle.ActiveColor == playerColor)
            {
                Debug.Log("Target color is same as player color");
                Guid circleId = targetCircle.Id;
                int tapCount = 0;

                circleTapCounts.TryGetValue(circleId, out tapCount);
                circleTapCounts[circleId] = ++tapCount;

                Debug.Log($"{circleId} has been tapped {tapCount} times");
                if (tapCount == maxTapCount)
                {
                    colorChangeTrigger.OnColorChange(colorChangeTrigger.NewColor, colorChangeTrigger.SourceCircle);
                    circleTapCounts.Remove(circleId);
                }
                return true;
            }
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            // Disable the "physical" PowerUp object
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().isKinematic = true;

            playerColor = coll.gameObject.GetComponent<CircleColor>().ActiveColor;

            PowerUpAcquiredPayload pwrUpAcquirePayload = new PowerUpAcquiredPayload(this);
            pwrUpAcquiredEvent.Invoke(pwrUpAcquirePayload);

            Debug.Log("MultiTap invoking PowerUpAcquiredEvent");
        }
    }
}
*/