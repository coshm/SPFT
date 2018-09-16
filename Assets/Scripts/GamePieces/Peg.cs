using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Peg : MonoBehaviour {

    private CircleCollider2D pegColl;
    private SpriteRenderer pegSprite;
    private ContactFilter2D contactFilter;
    private GameSettings gameSettings;

    void Awake() {
        pegColl = GetComponent<CircleCollider2D>();
        pegSprite = GetComponent<SpriteRenderer>();
        contactFilter = new ContactFilter2D();
    }
    
	void Start () {
        gameSettings = GameSettings.Instance;
    }
	
	void Update () {

    }

    public void Destroy(float timeToRespawn) {
        pegColl.enabled = false;
        pegSprite.enabled = false;

        StartCoroutine(Respawn(timeToRespawn));
    }

    private IEnumerator Respawn(float timeToRespawn) {
        yield return new WaitForSeconds(timeToRespawn);

        // Wait until Peg is not overlapping Puck
        while (IsPuckBlockingRespawn()) {
            yield return null;
        }

        pegSprite.enabled = true;
        pegColl.enabled = true;
    }

    /*private bool DoesPegOverlapPuck() {
        Collider2D[] overlapResults = new Collider2D[20];
        pegColl.OverlapCollider(contactFilter, overlapResults);
        foreach (Collider2D coll in overlapResults) {
            if (coll.gameObject.CompareTag(gameSettings.puckTag)) {
                return true;
            }
        }
        return false;
    }*/

    private bool IsPuckBlockingRespawn() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pegColl.radius + 0.1f);
        if (colliders == null || colliders.Length == 0) {
            return false;
        }

        foreach (Collider2D collider in colliders) {
            if (collider.CompareTag(gameSettings.puckTag)) {
                return true;
            }
        }
        return false;
    }
}
