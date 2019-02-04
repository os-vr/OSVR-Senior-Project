using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gestures {
    public class GestureEvent : UnityEvent<GestureMetaData> {
        public GestureEvent() : base() {}
        public GestureEvent(UnityAction<GestureMetaData> action) : base() {
            this.AddListener(action);
        }
    }
}
