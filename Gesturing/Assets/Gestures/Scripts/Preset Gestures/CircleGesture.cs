using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    /// <summary>
    /// A Circular Gesture
    /// </summary>
    /// <remarks> A proper aspect-ratio is necessary to complete this gesture. As such, an oval or ellipsoid shape will not be detected </remarks>
    public class CircleGesture : Gesture {

        /// <summary>
        /// Create a Circle Gesture with a specified tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance value for the circle gesture. Default is 0.4f</param>
        public CircleGesture(float tolerance = 0.4f) : base() {
            this.AddChecks(new List<Check> {
                new ArcCheck(new Vector3(0, 1, 0), 90, new Vector3(0,0,0), tolerance),
                new ArcCheck(new Vector3(1, 0, 0), 90, new Vector3(0,0,0), tolerance),
                new ArcCheck(new Vector3(0, -1, 0), 90, new Vector3(0,0,0), tolerance),
                new ArcCheck(new Vector3(-1, 0, 0), 90, new Vector3(0,0,0), tolerance),
            })
            .SetNormalizer(new FittedNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0), true));

        }
    }
}
