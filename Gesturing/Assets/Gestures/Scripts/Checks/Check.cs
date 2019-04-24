using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Gestures {

    /// <summary>
    /// An abstract class representing a single segment of a Gesture. 
    /// </summary>
    /// <remarks>
    /// Current implementations include Lines, Arcs, and Radii, all which deal with the position element of a GTransform
    /// </remarks>
    public abstract class Check {

        /// <summary>
        /// Determine whether a single GTransform passes the Check. 
        /// </summary>
        /// <param name="transform">GTransform data to compare against the specified Check</param>
        /// <returns> Returns a float (between 0 and 1) representing the distance from the center of a check, or -1 if the check fails </returns>
        public abstract float CheckPasses(GTransform transform);

    }

}