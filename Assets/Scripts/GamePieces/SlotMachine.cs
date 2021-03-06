﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using SPFT.EventSystem;
using SPFT.EventSystem.Events;
using SPFT.PowerUpSystem;
using SPFT.PowerUpSystem.PowerUps;
using SPFT.State;
using Random = UnityEngine.Random;

public class SlotMachine : SingletonBase<SlotMachine> {

    private const int DOWN = -1;
    private const int UP = 1;

    [SerializeField]
    public IDictionary<int, SpriteRenderer> slotMachineRenderers;

    private Queue<SlotMachineSlot> slotMachineSlots;
    private SlotMachineSlotPlanner slotPlanner;

    public Vector2 visibleReelYBounds;
    public Vector2 fullRotationsToCompleteRange;
    public float reelTransitionSpeed;
    public float reelSpinSpeed;

    private GameSettings gameSettings;
    private GameStateManager gameState;
    private PowerUpDataStore powerUpDataStore;

    private IPowerUp winningPowerUp;

    private int fullRotationsToComplete;
    private int reelRotationsCount;
    private int slotShiftCount;

    private float slotHeight;
    private int slotCount;

    void Awake() {
        // Manager references
        gameSettings = GameSettings.Instance;
        gameState = GameStateManager.Instance;
        powerUpDataStore = PowerUpDataStore.Instance;

        // Initializing various properties
        visibleReelYBounds = visibleReelYBounds == null ? gameSettings.visibleReelYBounds : visibleReelYBounds;
        fullRotationsToCompleteRange = fullRotationsToCompleteRange == null ? gameSettings.fullRotationsToCompleteRange : fullRotationsToCompleteRange;
        reelTransitionSpeed = reelTransitionSpeed == 0f ? gameSettings.reelTransitionSpeed : reelTransitionSpeed;
        reelSpinSpeed = reelSpinSpeed == 0f ? gameSettings.reelSpinSpeed : reelSpinSpeed;
        slotHeight = slotMachineRenderers[0].bounds.max.y - slotMachineRenderers[0].bounds.min.y;
        slotCount = slotMachineRenderers.Count;

        slotMachineSlots = new Queue<SlotMachineSlot>();
        for (int i = (slotCount - 1); i >= 0; i++) {
            slotMachineSlots.Enqueue(new SlotMachineSlot(slotMachineRenderers[i]));
        }
    }

    void Start() {
        powerUpGuids = powerUpDataStore.GetAllPowerUpGuids();
        slotPlanner = new SlotMachineSlotPlanner(powerUpGuids);
    }

    // Kick off the Slot Machine and select the PowerUp that the player 
    //   will receive once the Reel stops.
    public void PlaySlotsForPowerUp() {
        Guid winningPowerUpGuid = slotPlanner.SelectWinningPowerUp();
        GameObject powerUpPrefab = powerUpDataStore.GetPowerUpByGuid(winningPowerUpGuid);
        winningPowerUp = Instantiate(powerUpPrefab, Vector3.zero, Quaternion.identity).GetComponent<IPowerUp>();

        gameState.SetSlotMachineState(SlotMachineState.START_SPINNING);
        fullRotationsToComplete = Random.Range((int) fullRotationsToCompleteRange.x, (int) fullRotationsToCompleteRange.y);
    }

    void Update() {
        switch (gameState.SlotState) {
            case SlotMachineState.AT_REST:
                break;
            case SlotMachineState.START_SPINNING:
                MoveSlotReel(UP, reelTransitionSpeed);
                if (slotMachineSlots.ElementAt(0).HasMovedPassedY(UP, visibleReelYBounds.y)) {
                    gameState.SetSlotMachineState(SlotMachineState.SPINNING);
                    reelRotationsCount = 0;
                    slotShiftCount = 0;
                    // TODO: Add "blur" effect
                }
                break;
            case SlotMachineState.SPINNING:
                MoveSlotReel(DOWN, reelSpinSpeed);
                HandleSlotLooping();
                if (reelRotationsCount == fullRotationsToComplete) {
                    gameState.SetSlotMachineState(SlotMachineState.STOP_SPINNING);
                    // TODO: Remove "blur" effect
                }
                break;
            case SlotMachineState.STOP_SPINNING:
                MoveSlotReel(UP, reelTransitionSpeed);
                if (slotMachineSlots.ElementAt(0).HasMovedPassedY(DOWN, visibleReelYBounds.x)) {
                    gameState.SetSlotMachineState(SlotMachineState.AT_REST);

                    // Notify listeners that a PowerUp has been acquired.
                    PowerUpAcquiredEvent powerUpAcquiredEvent = new PowerUpAcquiredEvent() {
                        powerUp = winningPowerUp,
                        activationType = PowerUpActivationType.MANUAL
                    };
                    EventManager.Instance.NotifyListeners(powerUpAcquiredEvent);
                }
                break;
        }
    }

    // Move all the Slots on the Reel up by a given amount.
    private void MoveSlotReel(int direction, float reelSpeed) {
        float deltaY = Time.deltaTime * reelSpeed;
        foreach (SlotMachineSlot slotMachineSlot in slotMachineSlots) {
            slotMachineSlot.MoveSlot(direction, deltaY);
        }
    }

    // Handle when the bottom Slot has moved out of view and is reset to 
    //   the top of the Reel. Select a PowerUp for the new Slot on top.
    private void HandleSlotLooping() {
        // If the slot has passed out of view, loop it back to the top
        if (slotMachineSlots.ElementAt(0).IsOffCamera(visibleReelYBounds)) {
            // Move Slot to back of queue
            SlotMachineSlot loopingSlot = slotMachineSlots.Dequeue();
            slotMachineSlots.Enqueue(loopingSlot);

            // Update Slot position to match new order
            loopingSlot.MoveSlot(UP, visibleReelYBounds.y - visibleReelYBounds.x + slotHeight);

            // Update Slot PowerUp(Icon)
            Guid newSlotPowerUpGuid = slotPlanner.SelectPowerUpForNewSlot();
            Sprite newSlotSprite = powerUpDataStore.GetPowerUpIconByGuid(newSlotPowerUpGuid);
            loopingSlot.SetSlotSprite(newSlotSprite, newSlotPowerUpGuid);

            // Update counters to track the number of times we've shifted slots
            //   and made a full reel rotation
            slotShiftCount++;
            reelRotationsCount = slotShiftCount / slotCount;
        }
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~ SlotMachineSlot Wrapper Class ~~~~~~~~~~~~~~~~~~~~~~ */

    public class SlotMachineSlot {

        public Guid PowerUpGuid { get; private set; }

        private SpriteRenderer slotRenderer;
        private Transform slotTransform;

        public SlotMachineSlot(SpriteRenderer slotRenderer) {
            this.slotRenderer = slotRenderer;
            slotTransform = slotTransform.transform;
        }

        public void SetSlotSprite(Sprite slotSprite, Guid powerUpGuid) {
            slotRenderer.sprite = slotSprite;
            PowerUpGuid = powerUpGuid;
        }

        public void MoveSlot(int direction, float deltaY) {
            Vector2 newPos = slotTransform.position;
            newPos.y += Mathf.Sign(direction) * deltaY;
            slotTransform.position = newPos;
        }

        public bool IsOffCamera(Vector2 visibleReelYBounds) {
            return slotRenderer.bounds.min.y >= visibleReelYBounds.x 
                    || slotRenderer.bounds.max.y <= visibleReelYBounds.y;
        }

        public bool HasMovedPassedY(int direction, float yPos) {
            return Mathf.Sign(direction) == Mathf.Sign(slotTransform.position.y - yPos);
        }
    }

    public class SlotMachineSlotPlanner {

        private const int MIN_SELECTION_SIZE = 3;

        private int powerUpCount;

        private IList<Guid> powerUpGuids;

        // List of all PowerUps won in the SlotMachine in chronological order
        private IList<Guid> powerUpWinHistory;

        // Map of each PowerUp and the number of times it has been won
        private IDictionary<Guid, int> powerUpWinCounts;

        public SlotMachineSlotPlanner(IList<Guid> powerUpGuids) {
            powerUpCount = powerUpGuids.Count;
            this.powerUpGuids = powerUpGuids;
            powerUpWinHistory = new List<Guid>();
            powerUpWinCounts = new Dictionary<Guid, int>();
            foreach (Guid powerUpGuid in powerUpGuids) {
                powerUpWinCounts[powerUpGuid] = 0;
            }
        }

        // Select a smart "pseudo" random PowerUp that the player will receive once the spinning
        //   is over, being mindful of the number of times other PowerUps have been selected.
        public Guid SelectWinningPowerUp() {
            // Create a list of PowerUps ordered by number of times they've been selected before
            List<Guid> powerUpSelection = powerUpWinCounts.OrderBy(winCount => winCount.Value)
                    .Select(winCount => winCount.Key)
                    .ToList<Guid>();

            // Limit selection to the PowerUps picked least often
            int toIndex = powerUpCount > 2 * MIN_SELECTION_SIZE ? powerUpCount / 2 : MIN_SELECTION_SIZE;
            powerUpSelection.GetRange(0, toIndex);

            // Select random PowerUp from the selection
            Guid winningPowerUpGuid = powerUpSelection[Random.Range(0, powerUpSelection.Count)];

            // Record the winner for tracking purposes
            RecordWinningPowerUp(winningPowerUpGuid);

            return winningPowerUpGuid;
        }

        // Select a PowerUp for the next Slot on the Reel
        public Guid SelectPowerUpForNewSlot() {
            IList<Guid> powerUpsToAvoid = new List<Guid>();
            powerUpsToAvoid.Add(slotMachineSlots.ElementAt(slotCount - 2).PowerUpGuid);
            if (reelRotationsCount == fullRotationsToComplete - 1) {
                powerUpsToAvoid.Add(winningPowerUp.Id);
            }

            Guid nextPowerUpGuid;
            do {
                nextPowerUpGuid = powerUpGuids[Random.Range(0, powerUpGuids.Count)];
            } while (powerUpsToAvoid.Contains(nextPowerUpGuid));

            return nextPowerUpGuid;
        }

        // Track the order of PowerUps the player has won and how many times.
        public void RecordWinningPowerUp(Guid winningPowerUpGuid) {
            powerUpWinCounts[winningPowerUpGuid]++;
            winningPowerUpHistory.Add(winningPowerUpGuid);
        }

    }
}
