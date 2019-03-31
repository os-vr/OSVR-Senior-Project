using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    
    /// <summary>
    /// Multipurpose interface for normalizing data into a form that can be more easily recognized by the gesture detection algorithm
    /// </summary>
    public interface Normalizer {
        /// <summary>
        /// Normalize a set of points into a form that is easier for the system to interpret
        /// </summary>
        /// <param name="data">List of GTransform data to normalize</param>
        /// <returns>List of normalized GTransforms</returns>
        List<GTransform> Normalize(List<GTransform> data);
    }
}
