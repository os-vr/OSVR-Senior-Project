using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gestures {
    public class GTransformBuffer : IEnumerable<GTransform> {

        private int maxSize;
        private LinkedList<GTransform> data;

        public GTransformBuffer(int maxSize) {
            this.data = new LinkedList<GTransform>();
            this.maxSize = maxSize;
        }

        public void Enqueue(GTransform t) {
            if (data.Count >= maxSize) {
                return;
            }
            data.AddLast(t);

        }

        public int Size() {
            return data.Count;
        }

        public void Dequeue() {
            data.RemoveLast();
        }

        public void Clear() {
            data.Clear();
        }

        public IEnumerator<GTransform> GetEnumerator() {
            foreach (GTransform t in data) {
                yield return t;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public Vector3[] ToArray() {
            Vector3[] ret = new Vector3[data.Count];
            int counter = 0;
            foreach (GTransform g in data) {
                ret[counter++] = g.position;
            }
            return ret;
        }

    }
}
