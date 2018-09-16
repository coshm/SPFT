using UnityEngine;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;
using SPFT.State;
using SPFT.PowerUpSystem;
using SPFT.PowerUpSystem.PowerUps;

public class Puck : MonoBehaviour {
    
    private float startLaunchXPos;

    public Vector2 AimingStartPos { get; private set; }
    public Vector2 AimingEndPos { get; private set; }

    private Rigidbody2D puckBody;
    private GameSettings gameSettings;

    private Vector2 puckLastFrameVel;
    private Vector2 puckAcceleration;
    private bool shouldEmitCollisionEvents;

    void Awake() {
        Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        startLaunchXPos = screenCenter.x;
        shouldEmitCollisionEvents = false;

        puckBody = GetComponent<Rigidbody2D>();
        puckBody.bodyType = RigidbodyType2D.Kinematic;
    }

    void Start() {
        PowerUpLifeCycleManager.Instance.puck = this;
        gameSettings = GameSettings.Instance;

        EventManager.Instance.RegisterListener<PowerUpActivatedEvent>(OnPowerUpActivation);
        EventManager.Instance.RegisterListener<PowerUpExpiredEvent>(OnPowerUpExpired);
        EventManager.Instance.RegisterListener<BuyPuckEvent>(OnPuckPurchase);
    }

    void OnPowerUpActivation(PowerUpActivatedEvent pwrUpActivatedEvent) {
        if (pwrUpActivatedEvent.powerUp.GetType() == typeof(PegSmasher)) {
            Debug.Log("Activating PuckPegCollisionEvents.");
            shouldEmitCollisionEvents = true;
        }
    }

    void OnPowerUpExpired(PowerUpExpiredEvent pwrUpExpiredEvent) {
        if (pwrUpExpiredEvent.powerUp.GetType() == typeof(PegSmasher)) {
            Debug.Log("Deactivating PuckPegCollisionEvents.");
            shouldEmitCollisionEvents = false;
        }
    }

    void OnPuckPurchase(BuyPuckEvent buyPuckEvent) {
        // Turn off Puck physics before launch
        puckBody.bodyType = RigidbodyType2D.Kinematic;
        puckBody.velocity = Vector2.zero;
        AimingStartPos = Vector2.zero;
        AimingEndPos = Vector2.zero;
        transform.position = new Vector2(startLaunchXPos, gameSettings.puckLaunchY);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (shouldEmitCollisionEvents && collision.collider.CompareTag(gameSettings.pegTag)) {
            PuckPegCollisionEvent puckPegCollisionEvent = new PuckPegCollisionEvent() {
                puck = this,
                collision = collision
            };
            EventManager.Instance.NotifyListeners(puckPegCollisionEvent);
        }
    }

    void Update() {
        MainGameState ganeState = GameStateManager.Instance.State;
        switch(ganeState) {
            case MainGameState.PRE_LAUNCH:
                HandlePreLaunch();
                break;
            case MainGameState.LAUNCH_POSITIONING:
                HandleLaunchPositioning();
                break;
            case MainGameState.LAUNCH_AIMING:
                HandleLaunchAiming();
                break;
            case MainGameState.PUCK_DROPPING:
                HandlePuckDropping();
                break;
            case MainGameState.GAME_PAUSED:
                HandlePuckDropping();
                break;
        }
    }

    private void HandlePreLaunch() {
        if (Input.GetButtonDown(gameSettings.buyPuck)) {
            EventManager.Instance.NotifyListeners(new BuyPuckEvent());
        }
    }

    private void HandleLaunchPositioning() {
        Vector2 mousePosition = GetMousePosition();
        transform.position = new Vector2(mousePosition.x, gameSettings.puckLaunchY);

        if (Input.GetMouseButtonDown(0)) {
            AimingStartPos = mousePosition;
            PuckAimingEvent puckAimingEvent = new PuckAimingEvent() {
                aimingStartPos = AimingStartPos
            };
            EventManager.Instance.NotifyListeners(puckAimingEvent);
        }
    }

    private void HandleLaunchAiming() {
        Vector2 mousePosition = GetMousePosition();
        AimingEndPos = mousePosition;

        // Draw some kind of aiming arrow?

        if (Input.GetMouseButtonUp(0)) {
            // Enable physics
            puckBody.bodyType = RigidbodyType2D.Dynamic;

            // Calculate launch vector and apply it to the Puck
            Vector2 launchVector = AimingEndPos - AimingStartPos;
            Vector2 launchDir = launchVector / launchVector.magnitude;
            float launchPower = launchVector.magnitude > gameSettings.maxLaunchPower ? gameSettings.maxLaunchPower : launchVector.magnitude;
            puckBody.AddForce(launchDir * launchPower, ForceMode2D.Impulse);

            // Notify everyone we launched the Puck
            PuckLaunchEvent puckLaunchEvent = new PuckLaunchEvent() {
                aimingEndPos = mousePosition,
                launchPower = launchPower,
                launchDir = launchDir
            };
            EventManager.Instance.NotifyListeners(puckLaunchEvent);
        }
    }

    private void HandlePuckDropping() {
        ValidatePosition();

        // Calculate acceleration to pass along collision events
        puckAcceleration = (puckBody.velocity - puckLastFrameVel) / Time.deltaTime;
        puckLastFrameVel = puckBody.velocity;
    }

    private void ValidatePosition() {
        Vector2 xBounds = gameSettings.playAreaXBounds;
        Vector2 yBounds = gameSettings.playAreaYBounds;
        Vector2 puckPos = transform.position;
        if (puckPos.x < xBounds.x || puckPos.x > xBounds.y || puckPos.y < yBounds.x || puckPos.y > yBounds.y) {
            Debug.Log($"Puck went out of bounds. XBounds={xBounds}, YBounds={yBounds}, PuckPos={puckPos}");
            KillPuckEvent killPuckEvent = new KillPuckEvent() {
                causeOfDeath = CauseOfDeath.OUT_OF_BOUNDS,
                cashPenalty = gameSettings.outOfBoundsPenalty
            };
            EventManager.Instance.NotifyListeners(killPuckEvent);
        }
    }

    private void HandleGamePaused() {

    }

    public Vector2 GetPuckAcceleration() {
        return puckAcceleration;
    }

    private Vector2 GetMousePosition() {
        Vector3 worldCoords = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, 
            Input.mousePosition.y, 
            Camera.main.nearClipPlane));

        return new Vector2(worldCoords.x, worldCoords.y);
    }
}
