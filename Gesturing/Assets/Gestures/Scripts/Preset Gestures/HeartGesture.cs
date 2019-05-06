using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gestures{
    /// <summary>
    /// A Heart-Shaped Gesture
    /// </summary>
    public class HeartGesture : Gesture {

        /// <summary>
        /// Create a Heart Gesture with a specified tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance value for the lines and arcs of the heart. Default is 0.4f</param>
        public HeartGesture(float tolerance = 0.4f) : base() {
            this.AddChecks(new List<Check>{

                new ArcCheck(new Vector3(0, .5f, 0),        90, new Vector3(.5f, .5f, 0),       tolerance),
                new ArcCheck(new Vector3(.5f, 1.0f, 0),     90, new Vector3(.5f, .5f, 0),       tolerance),
                new ArcCheck(new Vector3(-1, .5f, 0),       90, new Vector3(-.5f, .5f, 0),      tolerance),
                new ArcCheck(new Vector3(-.5f, 1.0f, 0),    90, new Vector3(-.5f, .5f, 0),      tolerance),
                new LineCheck(new Vector3(0, -1f, 0),           new Vector3(.75f, -.25f, 0),    tolerance),
                new LineCheck(new Vector3(0, -1f, 0),           new Vector3(-.75f, -.25f, 0),   tolerance),
                new LineCheck(new Vector3(.75f, -.25f, 0),      new Vector3(1f, .5f, 0),        tolerance),
                new LineCheck(new Vector3(-.75f, -.25f, 0),     new Vector3(-1f, .5f, 0),       tolerance),

                new RadiusCheck(new Vector3(0, .5f, 0), .25f)
            })
            .SetNormalizer(new FittedNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)));

        }
    }
}
