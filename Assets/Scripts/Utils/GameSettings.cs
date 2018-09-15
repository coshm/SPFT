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
    public int maxStoredPowerUps = 5;

    public int maxPowerUpCapacity = 3;
    public float pwrUpPostDeactivationDelay = 5f;

    public float puckBounceMod = 1f;
    public float powerUpDuration = 30f;

    public float puckVelocityMod = 2f;
    public int maxPegBreaks = 2;
    public float stutterDuration = 0.1f; 
    public float pegRespawnDelay = 1f;
    public int maxExplosionCount = 3;

    public float teleportEnterYBound = 5f;
    public Vector2 teleportExitXBounds = new Vector2(-20f, 20f);
    public Vector2 teleportExitYBounds = new Vector2(20f, 10f);

    public float scoreMultiplier = 1.5f;
    public float scoreModDuration = 30f;
}
