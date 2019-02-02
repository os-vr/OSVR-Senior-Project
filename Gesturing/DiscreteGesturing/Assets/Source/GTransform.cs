using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gestures {
    public class GTransform {

        public Vector3 position;
        public Quaternion rotation;

        public GTransform(Vector3 pos, Quaternion quat) {
            position = pos;
            rotation = quat;
        }

        public GTransform Copy() {
            return new GTransform(position, rotation);
        }
    }
}

