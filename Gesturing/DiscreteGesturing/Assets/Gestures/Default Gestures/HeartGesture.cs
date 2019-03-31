using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gestures {
    /// <summary>
    /// A heart gesture set witha FittedNormalizer.
    /// </summary>
    public class HeartGesture : Gesture {
        
        public HeartGesture() : base() {
            this.AddOnceChecks(new List<Check>{

                new ArcCheck(new Vector3(0, .5f, 0), 90, new Vector3(.5f, .5f, 0)),
                new ArcCheck(new Vector3(.5f, 1.0f, 0), 90, new Vector3(.5f, .5f, 0)),
                new ArcCheck(new Vector3(-1, .5f, 0), 90, new Vector3(-.5f, .5f, 0)),
                new ArcCheck(new Vector3(-.5f, 1.0f, 0), 90, new Vector3(-.5f, .5f, 0)),
                new LineCheck(new Vector3(0, -1f, 0), new Vector3(.75f, -.25f, 0)),
                new LineCheck(new Vector3(0, -1f, 0), new Vector3(-.75f, -.25f, 0)),
                new LineCheck(new Vector3(.75f, -.25f, 0), new Vector3(1f, .5f, 0)),
                new LineCheck(new Vector3(-.75f, -.25f, 0), new Vector3(-1f, .5f, 0)),

                new RadiusCheck(new Vector3(0, .5f, 0), .25f)
            })
            .SetNormalizer(new FittedNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)));

        }
    }
}
