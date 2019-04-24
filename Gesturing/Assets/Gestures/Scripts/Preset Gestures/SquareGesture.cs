using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    /// <summary>
    /// A Square Gesture
    /// </summary>
    /// <remarks> A proper aspect-ratio is NOT necessary to complete this gesture. Any sufficiently rectangular shape will be detected </remarks>
    public class SquareGesture : Gesture {

        /// <summary>
        /// Create a Square Gesture with a specified tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance value for the edges of the square. Default is 0.4f</param>
        public SquareGesture(float tolerance = 0.4f) : base()
        {
            this.AddChecks(new List<Check> {
                new LineCheck(new Vector3(1, 1, 0), new Vector3(-1, 1, 0), tolerance),
                new LineCheck(new Vector3(-1, 1, 0), new Vector3(-1, -1, 0), tolerance),
                new LineCheck(new Vector3(-1, -1, 0), new Vector3(1, -1, 0), tolerance),
                new LineCheck(new Vector3(1, -1, 0), new Vector3(1, 1, 0), tolerance),

                new RadiusCheck(new Vector3(1, 1, 0), tolerance/2),
                new RadiusCheck(new Vector3(-1, 1, 0), tolerance/2),
                new RadiusCheck(new Vector3(-1, -1, 0), tolerance/2),
                new RadiusCheck(new Vector3(1, -1, 0), tolerance/2),
            })
            .SetNormalizer(new FittedNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)));

        }
    }
}

