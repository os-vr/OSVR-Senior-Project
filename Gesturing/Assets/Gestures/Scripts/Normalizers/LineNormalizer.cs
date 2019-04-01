using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {

    /// <summary>
    /// A Normalizer that only works to normalize single line segments. 
    /// </summary>
    /// <remarks> 
    /// The class has little use outside of detecting horizontal and vertical lines
    /// </remarks>
    public class LineNormalizer : Normalizer {

        public List<GTransform> Normalize(List<GTransform> data) {
            List<GTransform> normalizedData = new List<GTransform>();

            if(data.Count == 0) {
                return normalizedData;
            }

            Vector3 first = data[0].position;
            Vector3 last = data[data.Count - 1].position;

            Vector2 percentage = (last - first).normalized;

            for (int i = 0; i < data.Count; i++) {
                GTransform trans = data[i].Copy();
                trans.position -= first;
                trans.position.z = 0;

                normalizedData.Add(trans);
            }

            last = normalizedData[data.Count - 1].position;
            for (int i = 0; i < data.Count; i++) {
                GTransform trans = normalizedData[i];
                trans.position.x *= percentage.x * (2.0f / last.x);
                trans.position.y *= percentage.y * (2.0f / last.y);
                trans.position -= new Vector3(1 * percentage.x, 1 * percentage.y, 0);

            }


            return normalizedData;

        }

        public Vector2 Abs(Vector2 v) {
            v.x = Mathf.Abs(v.x);
            v.y = Mathf.Abs(v.y);
            return v;
        }


    }
}
