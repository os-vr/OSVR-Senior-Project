using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gestures {

    /// <summary>
    /// An interface representing a single piece of a gesture. 
    /// </summary>
    public abstract class Check {
        public abstract float CheckPasses(GTransform transform);
        public virtual void VisualizeCheck(Rect grid) {}

    }

}