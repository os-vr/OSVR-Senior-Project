using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHand : MonoBehaviour {

    public OVRInput.Controller controllerType;

	void Update () {
        transform.localPosition = OVRInput.GetLocalControllerPosition(controllerType);
        transform.localRotation = OVRInput.GetLocalControllerRotation(controllerType);
    }
}
