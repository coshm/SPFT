using UnityEngine;
using System;
using System.Collections;

public class PegExploder : MonoBehaviour, IPowerUp
{
    private const int INITIALIZE_ARGS = 5;

    public Guid Id { get; private set; }
    public Sprite Icon { get; private set; }
    public bool IsActive { get; private set; }

    public float timeToRespond;
    public int maxExplosionCount;
    public GameObject explosionEffect; // Change to some Animation object?

    private int pegsExplodedCount = 0;

    public void Initialize(params string[] args) {
        if (args == null || args.Length != INITIALIZE_ARGS) {
            throw new InvalidOperationException("Expected ${INITIALIZE_ARGS} args, got args=${args}");
        }

        Id = Guid.Parse(args[0]);
        Icon = Resources.Load<Sprite>(args[1]);
        timeToRespond = float.Parse(args[2]);
        maxExplosionCount = int.Parse(args[3]);
        explosionEffect = Resources.Load(args[4], typeof(GameObject)) as GameObject;
        IsActive = false;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update() {
        if (IsActive && Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag(GameSettings.Instance.pegTag)) {
                Peg peg = hit.collider.GetComponent<Peg>();
                Vector2 pegPosition = peg.transform.position;
                peg.Destroy(timeToRespond);

                // handle explosion effect

                pegsExplodedCount++;
                if (pegsExplodedCount >= maxExplosionCount) {
                    Deactivate();
                }
            }
        }
    }

    public void Activate() {
        IsActive = true;
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
