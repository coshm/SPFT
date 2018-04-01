/*using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public enum Player {
        P1,
        P2
    }

    public CircleMovement movement;
    public SpriteRenderer playerRenderer;

    public Player player;
    public float maxLaunchPower = 10.0f;

    private const string X_AXIS = "Horizontal";
    private const string Y_AXIS = "Vertical";
    private const string LAUNCH = "Launch";
    private const string POWER_UP = "PowerUp";
    
    void Start() {
        movement = GetComponent<CircleMovement>();
        if (movement == null) {
            throw new InvalidOperationException("Player's GameObject must have a Movement script.");
        }

        playerRenderer.sprite = SpriteHelper.GetPlayerIndicatorSprite(player);
    }

    // Update is called once per frame
    void Update() {
        float horizontalInput = Input.GetAxis(InputName(X_AXIS));
        float verticalInput = Input.GetAxis(InputName(Y_AXIS));
        float launchStr = Mathf.Sqrt(Mathf.Pow(horizontalInput, 2f) + Mathf.Pow(verticalInput, 2f));
        Vector2 launchDir = new Vector2(horizontalInput, verticalInput) / launchStr;

        // Update UI to show arrow in direction of current aim

        if (Input.GetButtonDown(InputName(LAUNCH)) && !movement.IsMoving) {
            Debug.Log($"Launced! H:{horizontalInput}, V:{verticalInput}, PWR:{launchStr}");
            // launch
            movement.MoveCircle(launchDir, launchStr * maxLaunchPower);
        }
        if (Input.GetButtonDown(InputName(POWER_UP))) {
            Debug.Log("PowerUp Botton Pressed");
            PowerUpManager.Instance.ActivatePowerUp();
        }
    }

    private string InputName(string input) {
        return $"{player.ToString()}_{input}";
    }
}
*/