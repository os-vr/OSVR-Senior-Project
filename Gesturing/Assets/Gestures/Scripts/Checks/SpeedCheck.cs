using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gestures {
    /// <summary>
    /// A check to ensure velocity between data points is greater or less than a certain velocity.
    /// </summary>
    public class SpeedCheck : Check {
        private float targetSpeed;
        private bool greaterThan;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetSpeed">The target speed to check against</param>
        /// <param name="greaterThan">Flag to indicate whether or not to check less or greater than target speed.</param>
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
