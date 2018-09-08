using UnityEngine;
using System;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;

public class Puck : MonoBehaviour {

    public enum LaunchState {
        LAUNCH_READY,
        LAUNCH_AIMING,
        LAUNCHED
    }
    
    private const float DEFAULT_MAX_LAUNCH_PWR = 10f;

    public LaunchState launchState;
    private float startLaunchXPos;
    public float launchYPos;
    public float maxLaunchPower;

    public Vector2 LaunchAimStart { get; private set; }
    public Vector2 LaunchAimEnd { get; private set; }

    private Rigidbody2D puckBody;

    void Awake() {
        if (launchYPos == 0f) {
            throw new InvalidOperationException("Must set valid Launch Height.");
        }

        if (maxLaunchPower == 0f) {
            maxLaunchPower = DEFAULT_MAX_LAUNCH_PWR;
        }

        Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        startLaunchXPos = screenCenter.x;

        puckBody = GetComponent<Rigidbody2D>();
        puckBody.bodyType = RigidbodyType2D.Kinematic;

        launchState = LaunchState.LAUNCH_READY;
    }

    void Start() {
        PowerUpManager.Instance.puck = this;
        EventManager.Instance.RegisterListener<PuckResetEvent>(OnPuckReset);
    }

    void Update() {
        if (launchState == LaunchState.LAUNCH_READY) {
            Vector2 mousePosition = GetMousePosition();
            transform.position = new Vector2(mousePosition.x, launchYPos);
        } else if (launchState == LaunchState.LAUNCH_AIMING) {
            Vector2 mousePosition = GetMousePosition();
            if (Input.GetMouseButtonDown(0)) {
                LaunchAimStart = mousePosition;
            }
            if (Input.GetMouseButton(0)) {
                LaunchAimEnd = mousePosition;
            }
            if (Input.GetMouseButtonUp(0)) {
                LaunchPuck();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll) {
        PuckCollisionTrigger puckCollTrigger = new PuckCollisionTrigger(coll, this);
        PowerUpManager.Instance.OnPowerUpTrigger(puckCollTrigger);
    }

    private void LaunchPuck() {
        // Enable physics
        launchState = LaunchState.LAUNCHED;
        puckBody.bodyType = RigidbodyType2D.Dynamic;

        // Calculate launch vector and apply it to the Puck
        Vector2 launchVector = LaunchAimEnd - LaunchAimStart;
        float launchPower = launchVector.magnitude > maxLaunchPower ? maxLaunchPower : launchVector.magnitude;
        Vector2 launchDir = launchVector / launchPower;
        puckBody.AddForce(launchDir * launchPower, ForceMode2D.Impulse);
    }

    public void OnPuckReset(PuckResetEvent puckResetEvent) {
        launchState = LaunchState.LAUNCH_READY;
        puckBody.bodyType = RigidbodyType2D.Kinematic;
        LaunchAimStart = Vector2.zero;
        LaunchAimEnd = Vector2.zero;
        transform.position = new Vector2(startLaunchXPos, launchYPos);
    }

    private Vector2 GetMousePosition() {
        Vector3 worldCoords = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, 
            Input.mousePosition.y, 
            Camera.main.nearClipPlane));

        return new Vector2(worldCoords.x, worldCoords.y);
    }
}
