using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    public class GestureMetaData {
        public Vector3 range;
        public Vector3 gestureCenter;
        //public float percentMatch;
        public string gestureName;
        public int dataPointCount;
        public float precision;

        public static GestureMetaData GetGestureMetaData(List<GTransform> transforms) {
            Vector3 min = transforms[0].position;
            Vector3 max = transforms[0].position;
            Vector3 centroid = new Vector3(0, 0, 0);
            for (int i = 0; i < transforms.Count; i++) {
                Vector3 pos = transforms[i].position;
                min = new Vector3(Math.Min(pos.x, min.x), Math.Min(pos.y, min.y), Math.Min(pos.z, min.z));
                max = new Vector3(Math.Max(pos.x, max.x), Math.Max(pos.y, max.y), Math.Max(pos.z, max.z));
                centroid += pos;
            }
            GestureMetaData ret = new GestureMetaData();

            ret.range = max - min;
            ret.gestureCenter = centroid / transforms.Count;
            ret.dataPointCount = transforms.Count;

            return ret;
        }

    }
}
