using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gestures {
    /// <summary>
    /// Wrapper class for a Unity Event that takes in GestureMetaData as a parameter
    /// </summary>
    public class GestureEvent : UnityEvent<GestureMetaData> {
        public GestureEvent() : base() {}
        public GestureEvent(UnityAction<GestureMetaData> action) : base() {
            this.AddListener(action);
        }
    }
}
