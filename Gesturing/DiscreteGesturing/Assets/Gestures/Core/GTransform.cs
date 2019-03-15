using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gestures {
    public class GTransform {

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;

        public GTransform(Vector3 pos, Quaternion quat, Vector3 vel) {
            position = pos;
            rotation = quat;
            velocity = vel;
        }

        public GTransform Copy() {
            return new GTransform(position, rotation, velocity);
        }
    }
}

