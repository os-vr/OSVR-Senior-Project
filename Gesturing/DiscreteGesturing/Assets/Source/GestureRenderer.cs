using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    public abstract class GestureRenderer : MonoBehaviour {
        public abstract void SetPositions(GTransformBuffer buffer);
    }
}
