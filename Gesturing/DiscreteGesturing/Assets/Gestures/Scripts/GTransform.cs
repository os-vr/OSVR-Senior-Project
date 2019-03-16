using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gestures {
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

        public GTransform Copy() {
            return new GTransform(position, rotation, velocity, time);
        }
    }
}

