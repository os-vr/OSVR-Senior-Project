using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Gestures {

    /// <summary>
    /// A Check to determine whether a point fits inside an Arc.
    /// </summary>
    public class ArcCheck : Check {

        /// <summary>
        /// Enum describing the orientation of the Arc along the three axes.
        /// </summary>
        public enum ARC_ORIENTATION {
            XY, /// <summary> Arc orientation is in the XY-plane </summary>
            YZ, /// <summary> Arc orientation is in the YZ-plane </summary>
            XZ, /// <summary> Arc orientation is in the XZ-plane </summary>
        };

        private float tolerance;
        private Vector3 startPosition;
        private float degrees;
        private Vector3 center;
        private float radius;
        private const float eps = 0.01f;
        private ARC_ORIENTATION orientation;

        /// <summary>
        /// Create an Arc Check used to determine whether a point fits inside the defined Arc. 
        /// </summary>
        /// <param name="startPosition">The starting position of the arc.</param>
        /// <param name="degrees">Number of degrees to extent. MUST be between -90 and 90 degrees, otherwise unpredictable consequences occur.</param>
        /// <param name="center">The center point of circle on which the Arc lies.</param>
        /// <param name="tolerance">The distance tolerance from which a point can be considered within the arc</param>
        /// <param name="orientation">ARC_ORIENTATION enum, specifying which plane the arc resides in</param>
        public ArcCheck(Vector3 startPosition, float degrees, Vector3 center, float tolerance = 0.4f, ARC_ORIENTATION orientation = ARC_ORIENTATION.XY) {
            this.startPosition = startPosition;
            this.degrees = Mathf.Clamp(degrees, -90.0f, 90.0f);
            this.center = center;
            this.radius = Vector3.Distance(center, startPosition);
            this.tolerance = tolerance;
            this.orientation = orientation;
        }


        private bool IsClockwise(Vector3 v1, Vector3 v2) {
            if (orientation == ARC_ORIENTATION.XY) {
                return Mathf.Sign(degrees) * (-v1.x * v2.y + v1.y * v2.x) >= 0;
            }

            if (orientation == ARC_ORIENTATION.XZ) {
                return Mathf.Sign(degrees) * (-v1.z * v2.x + v1.x * v2.z) >= 0;
            }

            return Mathf.Sign(degrees) * (-v1.y * v2.z + v1.z * v2.y) >= 0;
        }

        /// <summary>
        /// Determine whether a single GTransform fits inside the area defined by the Arc 
        /// </summary>
        /// <param name="g">GTransform to check position against</param>
        /// <returns> Returns a float (between 0 and 1) representing the distance from the center of the arc, or -1 if the check fails </returns>
        override public float CheckPasses(GTransform g) {

            Vector3 rotation = new Vector3(
                (orientation == ARC_ORIENTATION.YZ) ? 1 : 0, 
                (orientation == ARC_ORIENTATION.XZ) ? 1 : 0, 
                (orientation == ARC_ORIENTATION.XY) ? 1 : 0);
            Quaternion qrot = Quaternion.Euler(-degrees*rotation);

            Vector3 position = g.position;
            Vector3 direction = (position - center).normalized;
            Vector3 sectorStart = (startPosition - center).normalized;
            Vector3 sectorEnd = (qrot * sectorStart).normalized;
            float distance = Vector3.Distance(position, center);

            bool inArc = IsClockwise(sectorStart, direction) && !IsClockwise(sectorEnd, direction);
            bool prettyClose = (Vector3.Dot(sectorStart, direction) >= (1.0f - eps)) || (Vector3.Dot(sectorEnd, direction) >= (1.0f - eps));
            bool withinRadius = (distance < radius + tolerance/2.0f && distance > radius - tolerance/2.0f);

            if (withinRadius && (inArc || prettyClose)) {
                return Mathf.Abs((distance - radius))/(tolerance/2.0f);
            }
            return -1;
        }

    }
}
