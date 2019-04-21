using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    /// <summary>
    /// A square gesture set with the FittedNormalizer.
    /// </summary>
    public class SquareGesture : Gesture {
        public SquareGesture() : base()
        {
            this.AddOnceChecks(new List<Check> {
                new LineCheck(new Vector3(1, 1, 0), new Vector3(-1, 1, 0)),
                new LineCheck(new Vector3(-1, 1, 0), new Vector3(-1, -1, 0)),
                new LineCheck(new Vector3(-1, -1, 0), new Vector3(1, -1, 0)),
                new LineCheck(new Vector3(1, -1, 0), new Vector3(1, 1, 0)),

                new RadiusCheck(new Vector3(1, 1, 0)),
                new RadiusCheck(new Vector3(-1, 1, 0)),
                new RadiusCheck(new Vector3(-1, -1, 0)),
                new RadiusCheck(new Vector3(1, -1, 0)),
            })
            .SetNormalizer(new FittedNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)));

        }
    }
}

