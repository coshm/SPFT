using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SPFT.State;

public class SlotMachine : MonoBehaviour
{

    private static const int DOWN = -1;
    private static const int UP = 1;

    private static SlotMachine slotMachine;
    public static SlotMachine Instance
    {
        get
        {
            if (!slotMachine)
            {
                slotMachine = FindObjectOfType(typeof(SlotMachine)) as SlotMachine;
                if (!storeMgr)
                {
                    Debug.LogError("There needs to be one active SlotMachine script on a GameObject in your scene.");
                }
                else
                {
                    slotMachine.Init();
                }
            }
            return slotMachine;
        }
    }

    private void Init() { }

    [SerializeField]
    public IDictionary<int, SpriteRenderer> slotMachineRenderers;

    private Queue<SlotMachineSlot> slotMachineSlots;
    private IList<Guid> powerUpGuids;

    private IList<Guid> winningPowerUpHistory;
    private IDictionary<Guid, int> powerUpWinCounts;

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
        visibleReelYBounds = visibleReelYBounds ?? gameSettings.visibleReelYBounds;
        fullRotationsToCompleteRange = fullRotationsToCompleteRange ?? gameSettings.fullRotationsToCompleteRange;
        reelTransitionSpeed = reelTransitionSpeed == 0f ? gameSettings.reelTransitionSpeed : reelTransitionSpeed;
        reelSpinSpeed = reelSpinSpeed == 0f ? gameSettings.reelSpinSpeed : reelSpinSpeed;
        slotHeight = slotMachineRenderers[0].bounds.max.y - slotMachineRenderers[0].bounds.min.y;
        slotCount = slotMachineRenderers.Count;

        // Initializing data structures
        winningPowerUpHistory = new List<Guid>();
        powerUpWinCounts = new Dictionary<Guid, int>();
        slotMachineSlots = new Queue<SlotMachineSlot>();
        for (int i = (slotCount - 1); i >= 0; i++) {
            slotMachineSlots.Enqueue(new SlotMachineSlot(slotMachineRenderers[i]));
        }
    }

    void Start() {
        // Get a list of Guids for all PowerUp Prefabs we loaded
        powerUpGuids = powerUpDataStore.GetAllPowerUpGuids();
        foreach (Guid powerUpGuid in powerUpGuids) {
            powerUpWinCounts[powerUpGuid] = 0;
        }
    }

    // Kick off the Slot Machine and select the PowerUp that the player 
    //   will receive once the Reel stops.
    public void PlaySlotsForPowerUp() {
        Guid winningPowerUpGuid = SelectWinningPowerUp();
        GameObject powerUpPrefab = powerUpDataStore.GetPowerUpByGuid(winningPowerUpGuid).gameObject;
        winningPowerUp = Instantiate(powerUpPrefab, Vector3.zero, Quaternion.identity).GetComponent<IPowerUp>();

        gameState.SetSlotMachineState(SlotMachineState.START_SPINNING);
        fullRotationsToComplete = Random.Range(fullRotationsToCompleteRange.x, fullRotationsToCompleteRange.y);
    }

    // Select a smart "pseudo" random PowerUp that the player will receive once the spinning
    //   is over, being mindful of the number of times other PowerUps have been selected.
    private Guid SelectWinningPowerUp() {
        // Create a list of PowerUps ordered by number of times they've been selected before
        IList<Guid> potentialWinners = powerUpWinCounts.OrderBy(winCount => winCount.Value)
                .Select(winCount-> { winCount => winCount.Key })
                .ToList<Guid>();

        // Pick PowerUp from a subset of PowerUps that have been picked least often
        int powerUpSelectionSize = potentialWinners.Count < minPowerUpSelectionSize
                ? potentialWinners.Count : Math.Max(potentialWinners.Count / 2, minPowerUpSelectionSize);
        potentialWinners = potentialWinners.GetRange(0, powerUpSelectionSize);

        // Select random PowerUp from the selection
        Guid winningPowerUpGuid = potentialWinners[Random.Range(0, potentialWinners.Count)];

        // Record the winner for tracking purposes
        RecordWinningPowerUp(winningPowerUpGuid);

        return winningPowerUpGuid;
    }

    void Update() {
        switch (ganeState.SlotState) {
            case SlotMachineState.AT_REST:
                break;
            case SlotMachineState.START_SPINNING:
                MoveSlotReel(UP, reelTransitionSpeed);
                if (slotMachineSlots.ElementAt(0).HasMovedPassedY(UP, visibleReelYBounds.y)) {
                    gameState.SetSlotMachineState(SlotMachineState.SPINNING);
                    reelRotationsCount = 0f;
                    slotShiftCount = 0f;
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
                    SlotMachineCompleteEvent slotMachineCompleteEvent = new SlotMachineCompleteEvent() {
                        powerUp = winningPowerUp;
                    };
                    EventManager.Instance.NotifyListeners(slotMachineCompleteEvent);
                }
                break;
        }
    }

    // Move all the Slots on the Reel up by a given amount.
    private void MoveSlotReel(int direction, float reelSpeed) {
        float deltaY = Time.deltaTime * reelSpeed;
        foreach (SlotMachineSlot slotMachineSlot in slotMachineSlots) {
            slotMachineSlot.MoveY(direction, deltaY);
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
            Guid newSlotPowerUpGuid = SelectPowerUpForNewSlot();
            Sprite newSlotSprite = powerUpDataStore.GetPowerUpIconByGuid(newSlotPowerUpGuid);
            loopingSlot.SetSlotSprite(newSlotSprite, newSlotPowerUpGuid);

            // Update counters to track the number of times we've shifted slots
            //   and made a full reel rotation
            slotShiftCount++;
            reelRotationsCount = slotShiftCount / slotCount;
        }
    }

    // Select a PowerUp for the next Slot on the Reel
    private Guid SelectPowerUpForNewSlot() {
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
    private void RecordWinningPowerUp(Guid winningPowerUp) {
        int winCount = powerUpWinCounts[winningPowerUpGuid];
        powerUpWinCounts[winningPowerUpGuid] = winCount++;
        winningPowerUpHistory.Add(winningPowerUpGuid);
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~ SlotMachineSlot Wrapper Class ~~~~~~~~~~~~~~~~~~~~~~ */

    public class SlotMachineSlot {

        public Guid PowerUpGuid { get; private set; }

        private SpriteRenderer slotRenderer;
        private Transform slotTransform;

        public SlotMachineSlot(SpriteRenderer slotRenderer) {
            SlotRenderer = slotRenderer;
            slotTransform = slotTransform.transform;
        }

        public void SetSlotSprite(Sprite slotSprite, Guid powerUpGuid) {
            SlotRenderer.sprite = slotSprite;
            PowerUpGuid = powerUpGuid;
        }

        public void MoveSlot(int direction, float deltaY) {
            slotTransform.position.y += Mathf.Sign(direction) * deltaY;
        }

        public bool IsOffCamera(Vector2 visibleReelYBounds) {
            return SlotRenderer.bounds.min.y >= visibleReelYBounds.x 
                    || SlotRenderer.bounds.max.y <= visibleReelYBounds.y;
        }

        public bool HasMovedPassedY(int direction, float yPos) {
            return Mathf.Sign(direction) == Mathf.Sign(slotTransform.position.y - yPos);
        }
    }
}
