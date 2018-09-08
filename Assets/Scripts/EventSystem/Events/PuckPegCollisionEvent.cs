using UnityEngine;

namespace SPFT.EventSystem.Events {
    
    public struct PuckPegCollisionEvent : IEvent {
        public Puck puck;
        public Collision2D collision;
    }

}
