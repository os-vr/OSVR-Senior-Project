using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    /// <summary>
    /// A circle gesture set with a FittedNormalizer.
    /// </summary>
    public class CircleGesture : Gesture {
        public CircleGesture() : base() {
            this.AddOnceChecks(new List<Check> {
                new ArcCheck(new Vector3(0, 1, 0), 90, new Vector3(0,0,0)),
                new ArcCheck(new Vector3(1, 0, 0), 90, new Vector3(0,0,0)),
                new ArcCheck(new Vector3(0, -1, 0), 90, new Vector3(0,0,0)),
                new ArcCheck(new Vector3(-1, 0, 0), 90, new Vector3(0,0,0)),
            })
            .SetNormalizer(new FittedNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)));

        }
    }
}
