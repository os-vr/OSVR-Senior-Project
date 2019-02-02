using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    public interface Normalizer {
        List<GTransform> Normalize(List<GTransform> data);
    }
}
