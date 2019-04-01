using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extremely simple class for setting up a hand that moves with the position and rotation of an Oculus Touch Controller. 
/// </summary>
public class SimpleHand : MonoBehaviour {

    public OVRInput.Controller controllerType;

	void Update () {
        transform.localPosition = OVRInput.GetLocalControllerPosition(controllerType);
        transform.localRotation = OVRInput.GetLocalControllerRotation(controllerType);
    }
}
