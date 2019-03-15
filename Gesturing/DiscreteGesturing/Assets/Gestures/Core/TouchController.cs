using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures
{
    public class TouchController : IController
    {
        public OVRInput.Controller controllerType;
        public OVRInput.Button gestureActiveButton;
        public float vibrationFrequency = 0.2f;
        public float vibrationAmplitude = 0.2f;

        public override GTransform QueryGTransform()
        {
            return new GTransform(transform.position, transform.rotation, new Vector3(0,0,0), Time.time);
        }

        public override bool GestureActive()
        {
            bool isActive = OVRInput.Get(gestureActiveButton, controllerType);
            SetVibration(isActive);
            return isActive;
        }

        private void SetVibration(bool isActive)
        {
            int modifier = isActive ? 1 : 0;
            OVRInput.SetControllerVibration(vibrationFrequency * modifier, vibrationAmplitude * modifier, controllerType);
        }

    }
}
