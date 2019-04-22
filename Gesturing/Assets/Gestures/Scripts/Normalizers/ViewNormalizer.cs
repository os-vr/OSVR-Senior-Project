using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures{

    /// <summary>
    /// Normalizer implementation that takes the raw world-space GTransform data and rotates it about the user's position.
    /// </summary>
    /// <remarks> 
    /// The algorithm behind this class is not perfect yet. There are still problems with gestures that are too far above or below the user's eyeline
    /// </remarks>
    public class ViewNormalizer : Normalizer {

        private Transform userTransform; //transform for the user's gameObject to be used in normalization
        private Vector3 forward = new Vector3(0, 0, 1);

        /// <summary>
        /// Creates a ViewNormalizer based on the provided transform of the user's gameObject.
        /// </summary>
        /// <param name="userTransform"></param>
        public ViewNormalizer(Transform userTransform) {
            this.userTransform = userTransform;
        }

        /// <summary>
        /// Rotates the GTransforms to in front of the user gameObject.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<GTransform> Normalize(List<GTransform> data) {
            Vector3 centroid = new Vector3(0, 0, 0);
            int count = data.Count;
            List<GTransform> normalizedData = new List<GTransform>();

            if(count == 0) {
                return normalizedData;
            }

            for (int i = 0; i < data.Count; i++) {
                GTransform trans = data[i].Copy();
                centroid += trans.position;
                normalizedData.Add(trans);
            }


            centroid /= count;
            Vector3 userPosition = userTransform.position;
            Vector3 direction = (centroid - userPosition);

            float dp = Vector3.Dot(new Vector3(direction.x, 0, direction.z).normalized, direction.normalized);
            float angle = -Mathf.Sign(direction.y) * Mathf.Acos(dp);
            Quaternion rotation = Quaternion.AngleAxis(-angle * 360 / (2 * Mathf.PI), new Vector3(direction.z, 0, -direction.x).normalized);

            centroid = rotation * (centroid - userPosition) + userPosition;
            centroid = normalizedData[0].position;
            Vector3 direction2 = (centroid - userPosition).normalized;
            Quaternion rotation2 = Quaternion.FromToRotation(new Vector3(direction2.x, 0, direction2.z), forward);
            
            
            for (int i = 0; i < data.Count; i++) {
                GTransform trans = normalizedData[i];
                
                trans.position = rotation * (trans.position - userPosition) + userPosition;
                trans.position = rotation2 * (trans.position - userPosition) + userPosition;
            }

            return normalizedData;
        }

    }
}
