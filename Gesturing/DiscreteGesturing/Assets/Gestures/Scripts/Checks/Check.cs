using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gestures {

    /// <summary>
    /// An interface representing a single segment of a gesture. 
    /// </summary>
    /// <remarks>
    /// Current implementations include Lines, Arcs, and Radii, all which deal with the position element of a GTransform
    /// </remarks>
    public abstract class Check {
        public abstract float CheckPasses(GTransform transform);
        public virtual void VisualizeCheck(Rect grid) {}

    }

}