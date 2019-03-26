using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    /// <summary>
    /// A triangle gesture set with the FittedNormalizer.
    /// </summary>
    public class TriangleGesture : Gesture {
        public TriangleGesture() : base() {
            this.AddOnceChecks(new List<Check> {
                new LineCheck(new Vector3(-1, -1, 0), new Vector3(0, 1.0f, 0)),
                new LineCheck(new Vector3(0, 1.0f, 0), new Vector3(1, -1, 0)),
                new LineCheck(new Vector3(1, -1, 0),new Vector3(-1, -1, 0))
            })
            .SetNormalizer(new FittedNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)));

        }
    }
}