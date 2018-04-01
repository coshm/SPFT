using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SprintRenderer))]
public class Peg : MonoBehaviour {

    private Collider2D pegColl;
    private SpriteRenderer pegSprite;
    private ContactFilter2D contactFilter;

    void Awake() {
        pegColl = GetComponent<Collider2D>();
        pegSprite = GetComponent<SpriteRenderer>();
        contactFilter = new ContactFilter2D();
    }
    
	void Start () {
		
	}
	
	void Update () {
		
	}

    public void Explode(Vector2 impactDir, float timeToRespawn) {
        pegColl.enabled = false;
        pegSprite.enabled = false;

        // Set off particle system

        StartCoroutine(Respawn(timeToRespawn));
    }

    private IEnumerator Respawn(float timeToRespawn) {
        yield return new WaitForSeconds(timeToRespawn);

        // Wait until Peg is not overlapping Puck
        while (DoesPegOverlapPuck()) {
            yield;
        }

        pegSprite.enabled = true;
        pegColl.enabled = true;
    }

    private bool DoesPegOverlapPuck() {
        Collider2D overlapResults = new Collider2D[];
        pegColl.OverlapCollider(contactFilter, overlapResults);
        foreach (Collider2D coll in overlapResults) {
            if (coll.gameObject.tag == "Puck") {
                return true;
            }
        }
        return false;
    }
}
