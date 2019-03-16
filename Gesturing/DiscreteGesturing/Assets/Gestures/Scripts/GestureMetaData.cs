using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    public class GestureMetaData {
        public Vector3 scale;
        public Vector3 centroid;
        public string name = "NO_GESTURE";
        public int pointCount;
        public float time;
        public float precision;
        public float averageSpeed = 0.0f;

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
