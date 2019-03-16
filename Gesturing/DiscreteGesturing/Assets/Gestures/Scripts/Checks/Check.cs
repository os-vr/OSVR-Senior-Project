using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gestures {

    public abstract class Check {
        public abstract float CheckPasses(GTransform transform);
        public virtual void VisualizeCheck(Rect grid) {}

    }

}