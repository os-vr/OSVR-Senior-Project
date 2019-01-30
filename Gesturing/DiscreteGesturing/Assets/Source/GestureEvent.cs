using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GestureEvent : UnityEvent<GestureMetaData> {

	public GestureEvent(UnityAction<GestureMetaData> action) : base()
    {
        this.AddListener(action);
    }
}
