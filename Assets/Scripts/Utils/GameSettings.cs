using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour {

    private static GameSettings gameSettings;
    public static GameSettings Instance {
        get {
            if (!gameSettings) {
                gameSettings = FindObjectOfType(typeof(GameSettings)) as GameSettings;
                if (!gameSettings) {
                    Debug.LogError("There needs to be one active GameSettings script on a GameObject in your scene.");
                }
            }
            return gameSettings;
        }
    }

    // Input Names
    public string vertical = "Vertical";
    public string horizontal = "Horizontal";
    public string action = "Action";
    public string buyPuck = "BuyPuck";
    public string buyPowerUp = "BuyPowerUp";
    public string activatePowerUp = "ActivatePowerUp";

    // Tag Names
    public string puckTag = "Puck";
    public string pegTag = "Peg";

    public int startingCash = 1000;
    public float puckLaunchY = 24f;
    public float maxLaunchPower = 10f;
    public Vector2 playAreaXBounds = new Vector2(-30f, 30f);
    public Vector2 playAreaYBounds = new Vector2(-50f, 50f);

    public int outOfBoundsPenalty = 50;
    public int puckCost = 100;
    public int powerUpCost = 100;

    public int maxPowerUpCapacity = 3;

    public float puckBounceMod = 1f;
    public float powerUpDuration = 30f;

    public float puckVelocityMod = 2f;
    public int maxPegBreaks = 2;
    public float stutterDuration = 0.1f; 
    public float pegRespawnDelay = 1f; 
}
