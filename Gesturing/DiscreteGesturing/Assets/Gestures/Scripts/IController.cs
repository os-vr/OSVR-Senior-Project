using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures
{
    /// <summary>
    /// Abstract class which is responsible for tracking transform data for a VR headset. 
    /// </summary>
    /// <remarks>
    /// Although we have only provided support for Oculus Rift Touch Controllers, any headset may be used if an appropriate concrete implementation of this class is created
    /// </remarks>
    public abstract class IController : MonoBehaviour {

        /// <summary>
        /// Generate the data representing the necessary transform data for a given controller
        /// </summary>
        /// <returns>Returns `GTransform` instance containing transform data</returns>
        public abstract GTransform QueryGTransform();

        /// <summary>
        /// This method will query the controller to determine whether the gesture should be tracked or not.
        /// </summary>
        /// <remarks>
        /// The most common implementation of this method simply returns whether a specific button is pressed.
        /// </remarks>
        /// <returns>Return `true` if a gesture should be active, `false` otherwise</returns>
        public abstract bool GestureActive();

    }
}
