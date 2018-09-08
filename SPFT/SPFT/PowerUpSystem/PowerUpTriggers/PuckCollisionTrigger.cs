using UnityEngine;

public class PuckCollisionTrigger : IPowerUpTrigger {

    public Collision2D Coll { get; private set; }
    public Puck Puck { get; private set; }

    public PuckCollisionTrigger(Collision2D coll, Puck puck) {
        Coll = coll;
        Puck = puck;
    }
}
