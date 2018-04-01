using UnityEngine;
using System;

public class Puck : MonoBehaviour {
    
    private const float DEFAULT_MAX_LAUNCH_PWR = 10f;

    public bool LaunchReady { get; private set; }
    private float startLaunchXPos;
    public float launchYPos;
    public float maxLaunchPower;

    public Vector2 LaunchAimStart { get; private set; }
    public Vector2 LaunchAimEnd { get; private set; }

    private Rigidbody2D puckBody;
    public delegate void OnPuckCollision(Collision2D coll);

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

        LaunchReady = true;
    }


    void Start() {
        PowerUpManager.Instance.puck = this;
        EventManager.Instance.RegisterListenerWithPayload<PuckResetEvent>(OnPuckReset);
    }

    void Update() {
        if (LaunchReady) {
            Vector2 mousePosition = GetMousePosition();

            if (LaunchAimStart == Vector2.zero) {
                transform.position = new Vector2(mousePosition.x, launchYPos);
            }
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
        PuckCollisionTrigger puckCollTrigger = new PuckCollisionTrigger(coll, this, HandleCollision);
        bool wasHandled = PowerUpManager.Instance.OnPowerUpTrigger(puckCollTrigger);
        if (!wasHandled) {
            HandleCollision(coll);
        }
    }

    public void HandleCollision(Collision2D coll) {
        // stuff
    }

    private void LaunchPuck() {
        // Enable physics
        LaunchReady = false;
        puckBody.bodyType = RigidbodyType2D.Dynamic;

        // Calculate launch vector and apply it to the Puck
        Vector2 launchVector = LaunchAimEnd - LaunchAimStart;
        float launchPower = launchVector.magnitude > maxLaunchPower ? maxLaunchPower : launchVector.magnitude;
        Vector2 launchDir = launchVector / launchPower;
        puckBody.AddForce(launchDir * launchPower, ForceMode2D.Impulse);
    }

    public void OnPuckReset(IEventPayload genericPayload) {
        if (genericPayload.GetType() == typeof(PuckResetPayload)) {
            LaunchReady = true;
            puckBody.bodyType = RigidbodyType2D.Kinematic;
            LaunchAimStart = Vector2.zero;
            LaunchAimEnd = Vector2.zero;
            transform.position = new Vector2(startLaunchXPos, launchYPos);
        }
    }

    private Vector2 GetMousePosition() {
        Vector3 worldCoords = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, 
            Input.mousePosition.y, 
            Camera.main.nearClipPlane));

        return new Vector2(worldCoords.x, worldCoords.y);
    }
}
