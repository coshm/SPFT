using UnityEngine;

namespace SPFT.EventSystem.Events {

    public struct PuckLaunchEvent : IEvent {
        public Vector2 aimingEndPos;
        public float launchPower;
        public Vector2 launchDir;
    }

}
