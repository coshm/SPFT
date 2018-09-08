using UnityEngine;
using System;
using System.Collections;

public class ReorderStoredPowerUps : MonoBehaviour, IPowerUp
{
    private const int INITIALIZE_ARGS = 4;

    public Guid Id { get; private set; }
    public Sprite Icon { get; private set; }
    public bool IsActive { get; private set; }

    private PowerUpManager pwrUpMgr;
    private int[] reorderedIconIndices;

    public void Initialize(params string[] args) {
        if (args == null || args.Length != INITIALIZE_ARGS) {
            throw new InvalidOperationException("Expected ${INITIALIZE_ARGS} args, got args=${args}");
        }
        
        Id = Guid.Parse(args[0]);
        Icon = Resources.Load<Sprite>(args[1]);
        IsActive = false;
    }


    // Use this for initialization
    void Start() { }

    // Update is called once per frame
    void Update() {
        if (IsActive && Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && pwrUpMgr.IsAPowerUpIcon(hit.collider.gameObject)) {
                int iconIdx = pwrUpMgr.GetIndexFromIconTag(hit.collider.gameObject);
                reorderedIconIndices[reorderedIconIndices.Length] = iconIdx;

                if (reorderedIconIndices.Length == pwrUpMgr.StoredPowerUpCount) {
                    pwrUpMgr.SetStoredPowerUpOrder(reorderedIconIndices);
                    Deactivate();
                }
            }
        }
    }

    public void Activate() {
        IsActive = true;
        if (pwrUpMgr.StoredPowerUpCount == 0) {
            Deactivate();
        } else {
            reorderedIconIndices = new int[pwrUpMgr.StoredPowerUpCount];
        }
    }

    public void Deactivate() {
        IsActive = false;
        Destroy(this);
    }

    public bool IsBlockingPowerUpActivation(IPowerUp pwrUp) {
        return pwrUp.GetType() == typepf(this);
    }

    public bool OnPowerUpTrigger(IPowerUpTrigger pwrUpTrigger) {
        return false;
    }
}
