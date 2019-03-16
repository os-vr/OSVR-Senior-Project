using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gestures {

    public class SpeedCheck : Check {
        private float targetSpeed;
        private bool greaterThan;

        public SpeedCheck(float targetSpeed, bool greaterThan) {
            this.targetSpeed = targetSpeed;
            this.greaterThan = greaterThan;
        }

        override public float CheckPasses(GTransform gTransform) {
            float speed = gTransform.velocity.magnitude;
            return  (speed < targetSpeed) ? 1 : -1;
        }

        public override void VisualizeCheck(Rect grid) {
           
        }

    }
}
