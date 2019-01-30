using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : IController
{

    public OVRInput.Controller controllerType;
    public OVRInput.Button gestureActiveButton;
    public float vibrationFrequency = 0.2f;
    public float vibrationAmplitude = 0.2f;

    public override void UpdateController()
    {
        transform.localPosition = OVRInput.GetLocalControllerPosition(controllerType);
        transform.localRotation = OVRInput.GetLocalControllerRotation(controllerType);
    }

    public override GTransform QueryGTransform()
    {
        return new GTransform(transform.position, transform.rotation);
    }

    public override bool GestureActive()
    {
        return true;
    }

    private void SetVibration(bool isActive)
    {
        int modifier = isActive ? 1 : 0;
        OVRInput.SetControllerVibration(vibrationFrequency * modifier, vibrationAmplitude * modifier, controllerType);
    }

}