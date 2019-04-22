using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures
{
    /// <summary>
    /// A concrete implementation of the IController class, targeted at the Oculus Rift Touch Controllers
    /// </summary>
    public class TouchController : IController
    {
        /// <summary> The Touch Controller to use. Typically LTouch or RTouch for left and right hands. </summary>
        public OVRInput.Controller controllerType;

        /// <summary> The controller button which will activate a gesture </summary>
        public OVRInput.Button gestureActiveButton;

        /// <summary> The frequency of the controller vibration when the gestureActiveButton is pressed </summary>
        public float vibrationFrequency = 0.2f;

        /// <summary> The amplitude of the controller vibration when the gestureActiveButton is pressed </summary>
        public float vibrationAmplitude = 0.2f;

        /// <summary>
        /// Generate the data representing the position, rotation, velocity, and timing data for the controller
        /// </summary>
        /// <remark>
        /// Velocity depends on OVRInput.GetLocalControllerVelocity.
        /// </remark>
        /// <returns>Returns `GTransform` instance containing transform data</returns>
        public override GTransform QueryGTransform()
        {
            Vector3 vel = OVRInput.GetLocalControllerVelocity(controllerType);
            return new GTransform(transform.position, transform.rotation, vel, Time.time);
        } 

        /// <summary>
        /// This method will query the controller to determine whether the gesture activation button is pressed on the correct controller
        /// </summary>
        /// <returns>Return `true` if the user is pressing the gesture button, `false` otherwise</returns>
        public override bool GestureActive()
        {
            bool isActive = OVRInput.Get(gestureActiveButton, controllerType);
            SetVibration(isActive);
            return isActive;
        }

        /// <summary>
        /// Set the controller vibration
        /// </summary>
        /// <remarks>This functionality notifies the user that a gesture is active. It can be disabled by setting the frequency and amplitude of vibration to 0</remarks>
        /// <param name="isActive">Boolean to indicate whether to enable or disable vibrations</param>
        private void SetVibration(bool isActive)
        {
            int modifier = isActive ? 1 : 0;
            OVRInput.SetControllerVibration(vibrationFrequency * modifier, vibrationAmplitude * modifier, controllerType);
        }
    }
}

