using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    /// <summary>
    /// A data container class to store extra meta data about the completed gesture
    /// </summary>
    public class GestureMetaData {
        /// <summary> The scale of the gesture in world-space </summary>
        public Vector3 scale;

        /// <summary> The center of the gesture in world-space </summary>
        public Vector3 centroid;

        /// <summary> The name of the completed gesture. </summary>
        public string name = "NO_GESTURE";

        /// <summary> The number of points used to draw the gesture </summary>
        public int pointCount;

        /// <summary> The total time it took to draw the gesture </summary>
        public float time;

        /// <summary> A measure of how close a gesture was to the exact definition. A perfect gesture has a precision of 0, a gesture that barely passes has a precision of 1 </summary>
        public float precision;

        /// <summary> The average speed of the user's hand over the course of the gesture </summary>
        public float averageSpeed = 0.0f;

        /// <summary>
        /// Static helper method to convert a list of transforms into a partially complete GestureMetaData object
        /// </summary>
        /// <param name="transforms">The list of raw GTransform data</param>
        /// <returns>Returns a `GestureMetaData` instance with fields populated</returns>
        public static GestureMetaData GetGestureMetaData(List<GTransform> transforms) {
            Vector3 min = transforms[0].position;
            Vector3 max = transforms[0].position;
            Vector3 centroid = new Vector3(0, 0, 0);
            float speed = 0.0f;
            int count = transforms.Count;
            for (int i = 0; i < count; i++) {
                Vector3 pos = transforms[i].position;
                min = new Vector3(Math.Min(pos.x, min.x), Math.Min(pos.y, min.y), Math.Min(pos.z, min.z));
                max = new Vector3(Math.Max(pos.x, max.x), Math.Max(pos.y, max.y), Math.Max(pos.z, max.z));
                centroid += pos;

                speed += transforms[i].velocity.magnitude;
            }
            GestureMetaData ret = new GestureMetaData();

            ret.scale = max - min;
            ret.centroid = centroid / count;
            ret.pointCount = count;
            ret.time = transforms[count - 1].time - transforms[0].time;
            ret.averageSpeed = speed / count;

            return ret;
        }

    }
}
