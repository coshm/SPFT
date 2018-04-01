﻿using Random = UnityEngine.Random;
using ActivationType = PowerUpAcquiredPayload.ActivationType;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {

    private const string BUY_PWR_UP_KEY = "space";
    private const string ACTIVATE_PWR_UP_KEY = "a";
    
    private static PowerUpManager powerUpMgr;
    public static PowerUpManager Instance {
        get {
            if (!powerUpMgr) {
                powerUpMgr = FindObjectOfType(typeof(PowerUpManager)) as PowerUpManager;
                if (!powerUpMgr) {
                    Debug.LogError("There needs to be one active PowerUpManager script on a GameObject in your scene.");
                } else {
                    powerUpMgr.Init();
                }
            }
            return powerUpMgr;
        }
    }

    public Puck puck;

    [SerializeField]
    public List<GameObject> allPowerUpPrefabs;
    [SerializeField]
    public Queue<IPowerUp> storedPowerUps;
    [SerializeField]
    public List<IPowerUp> activePowerUps;
    [SerializeField]
    public List<IPowerUp> expiredPowerUps;

    void Init() {
        if (puck == null) {
            throw new InvalidOperationException("Puck cannot be null.");
        }
    }

    void Awake() {
        if (allPowerUpPrefabs == null || allPowerUpPrefabs.Count == 0) {
            throw new InvalidOperationException("There must be at least one PowerUp in allPowerUps.");
        }

        storedPowerUps = new Queue<IPowerUp>();
        activePowerUps = new List<IPowerUp>();
        expiredPowerUps = new List<IPowerUp>();
    }

    void Start() {
        EventManager.Instance.RegisterListenerWithPayload<PowerUpAcquiredEvent>(OnPowerUpAcquired);
        EventManager.Instance.RegisterListenerWithPayload<PowerUpExpiredEvent>(OnPowerUpExpired);
    }

    void Update() {
        if (Input.GetButtonDown(BUY_PWR_UP_KEY)) {
            GetRandomPowerUp();
        }else if (Input.GetButtonDown(ACTIVATE_PWR_UP_KEY)) {
            ActivateStoredPowerUp();
        }
    }

    public void ActivateStoredPowerUp() {
        // If we don't have any stored PowerUps, then nothing to do
        if (storedPowerUps.Count == 0) {
            Debug.Log("No Stored PowerUps");
            return;
        }

        // Check if any active PowerUps are blocking the 
        // activation of the next PowerUp
        IPowerUp nextPwrUp = storedPowerUps.Peek();
        bool isBlocked = false;
        activePowerUps.ForEach(delegate (IPowerUp pwrUp) {
            isBlocked |= pwrUp.IsBlockingPowerUpActivation(nextPwrUp);
        });

        // If next PowerUp is not blocked, we can activate  
        // it and remove it from the stored PowerUp queue
        if (!isBlocked) {
            storedPowerUps.Dequeue();
            ActivatePowerUp(nextPwrUp);
        }
    }

    public void ActivatePowerUp(IPowerUp powerUp) {
        powerUp.Activate();
        activePowerUps.Add(powerUp);
    }

    public void DeactivatePowerUp(IPowerUp powerUp) {
        activePowerUps.Remove(powerUp);
        expiredPowerUps.Add(powerUp);
    }

    public IPowerUp GetRandomPowerUp() {
        // Get random PowerUp prefab and instantiate it
        GameObject powerUpPrefab = allPowerUpPrefabs[Random.Range(0, allPowerUpPrefabs.Count)];
        return Instantiate(powerUpPrefab, Vector3.zero, Quaternion.identity).GetComponent<IPowerUp>();
    }

    /* ~~~~~~~~~~~~~~~~~~~~ Unity Event Handlers ~~~~~~~~~~~~~~~~~~~~ */

    public void OnPowerUpAcquired(IEventPayload genericPayload) {
        if (genericPayload.GetType() == typeof(PowerUpAcquiredPayload)) {
            PowerUpAcquiredPayload pwrUpAcquiredPayload = (PowerUpAcquiredPayload)genericPayload;
            if (pwrUpAcquiredPayload.Type == ActivationType.IMMEDIATE) {
                ActivatePowerUp(pwrUpAcquiredPayload.PowerUp);
            } else {
                storedPowerUps.Enqueue(pwrUpAcquiredPayload.PowerUp);
            }
        }
    }

    public void OnPowerUpExpired(IEventPayload genericPayload) {
        if (genericPayload.GetType() == typeof(PowerUpExpiredPayload)) {
            PowerUpExpiredPayload pwrUpExpiredPayload = (PowerUpExpiredPayload)genericPayload;
            activePowerUps.Remove(pwrUpExpiredPayload.PowerUp);
        }
    }

    /* ~~~~~~~~~~~~~~~~~~ PowerUp Trigger Handlers ~~~~~~~~~~~~~~~~~~ */

    public bool OnPowerUpTrigger(IPowerUpTrigger pwrUpTrigger) {
        bool wasHandled = false;
        activePowerUps.ForEach(delegate (IPowerUp pwrUp) {
            wasHandled |= pwrUp.OnPowerUpTrigger(pwrUpTrigger);
        });
        return wasHandled;
    }
}
