using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    
    /// <summary>
    /// Fitted Normalizer will be the standard normalizer for any 2D or 3D Gestures.  
    /// </summary>
    public class FittedNormalizer : Normalizer {

        private Vector3 bottomLeft, topRight;
        private bool maintainAspectRatio;

        public FittedNormalizer() : this(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)) { }

        /// <summary>
        /// Create a new Fitted Normalizer with specific bounds
        /// </summary>
        /// <param name="bottomLeft">Vector3 defining the bottomleft-most coordinate of the bounding box encompassing the Gesture</param>
        /// <param name="topRight">Vector3 defining the topright-most coordinate of the bounding box encompassing the Gesture</param>
        /// <param name="maintainAspectRatio">`True` if the Gesture must be performed such that the aspect ratio of the drawn figure matches the aspect ratio of the Gesture definition. Most of the time, this should be `False`</param>
        public FittedNormalizer(Vector3 bottomLeft, Vector3 topRight, bool maintainAspectRatio = false) {
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.maintainAspectRatio = maintainAspectRatio;
        }

        public delegate float MinMaxDelegate(float a, float b);
        private Vector3 ApplyPredToVector(Vector3 a, Vector3 b, MinMaxDelegate pred) {
            return new Vector3(pred(a.x, b.x), pred(a.y, b.y), pred(a.z, b.z));
        }

        private Vector3 Inverse(Vector3 vec) {
            float ix = (vec.x != 0) ? 1.0f / vec.x : 0.0f;
            float iy = (vec.y != 0) ? 1.0f / vec.y : 0.0f;
            float iz = (vec.z != 0) ? 1.0f / vec.z : 0.0f;
            return new Vector3(ix, iy, iz);
        }


        public List<GTransform> Normalize(List<GTransform> data) {
            List<GTransform> normalizedData = new List<GTransform>();

            int count = data.Count;
            if(count == 0) {
                return normalizedData;
            }

            Vector3 range = new Vector3(0, 0, 0);
            Vector3 min = data[0].position;
            Vector3 max = data[0].position;


            for (int i = 0; i < data.Count; i++) {
                GTransform trans = data[i].Copy();
                Vector3 pos = trans.position;

                min = ApplyPredToVector(min, pos, Mathf.Min);
                max = ApplyPredToVector(max, pos, Mathf.Max);

                normalizedData.Add(trans);
            }

            range = max - min;

            if (maintainAspectRatio) {
                float xRange = (topRight.x - bottomLeft.x);
                float yRange = (topRight.y - bottomLeft.y);
                float zRange = (topRight.z - bottomLeft.z);

                if (xRange != 0) {
                    range.y = (yRange / xRange) * range.x;
                    range.z = (zRange / xRange) * range.x;
                }
            }

            for (int i = 0; i < data.Count; i++) {
                GTransform trans = normalizedData[i];

                trans.position -= min;

                Vector3 v = (topRight - bottomLeft);
                v.Scale(Inverse(range));
                trans.position.Scale(v);

                trans.position += bottomLeft;
            }

            return normalizedData;
        }

    }
}
