using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gestures {
    /// <summary>
    /// A Triangular Gesture.
    /// </summary>
    public class TriangleGesture : Gesture {

        /// <summary>
        /// Create a Triangle Gesture with a specified tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance value for the edges of the triangle. Default is 0.4f</param>
        public TriangleGesture(float tolerance = 0.4f) : base() {
            this.AddChecks(new List<Check> {
                new LineCheck(new Vector3(-1, -1, 0), new Vector3(0, 1.0f, 0)),
                new LineCheck(new Vector3(0, 1.0f, 0), new Vector3(1, -1, 0)),
                new LineCheck(new Vector3(1, -1, 0),new Vector3(-1, -1, 0))
            })
            .SetNormalizer(new FittedNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)));

        }
    }
}