using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gestures {

    /// <summary>
    /// Data class to contain all necessary information related to a controller transform
    /// </summary>
    public class GTransform {

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public float time;

        public GTransform(Vector3 pos, Quaternion quat, Vector3 vel, float t) {
            position = pos;
            rotation = quat;
            velocity = vel;
            time = t;
        }

        /// <summary>
        /// Create a copy of the GTransform instance
        /// </summary>
        /// <returns>
        /// Return new `GTransform` with the same data as the object this method was called on.
        /// </returns>
        public GTransform Copy() {
            return new GTransform(position, rotation, velocity, time);
        }
    }
}

