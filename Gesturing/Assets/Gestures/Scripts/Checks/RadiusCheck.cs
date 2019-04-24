using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Gestures {

    /// <summary>
    /// A Check to check if a point is within a certain radius of another point.
    /// </summary>
    public class RadiusCheck : Check {

        private Vector3 position;
        private float radius;

        /// <summary>
        /// Create a Radius Check used to determine whether a point fits inside the defined circular area. 
        /// </summary>
        /// <param name="position">The center of the circle</param>
        /// <param name="radius">A radius tolerance away from the center</param>
        public RadiusCheck(Vector3 position, float radius = 0.4f) {
            this.position = position;
            this.radius = radius;
        }

        /// <summary>
        /// Determine whether a single GTransform fits inside the area defined by the circle 
        /// </summary>
        /// <param name="g">GTransform to check position against</param>
        /// <returns> Returns a float (between 0 and 1) representing the distance from the center of the circle, or -1 if the check fails </returns>
        override public float CheckPasses(GTransform gTransform) {
            float distance = Vector3.Distance(gTransform.position, position);
            if (distance > radius) {
                return -1;
            }
            return 1;
        }


    }
}
