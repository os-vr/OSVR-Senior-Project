using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gestures {

    /// <summary>
    /// Internal class which can behave as either a set-size array or a circular array. 
    /// </summary>
    public class GTransformBuffer : IEnumerable<GTransform> {

        private int maxSize;
        private bool circular = false;
        private LinkedList<GTransform> data;

        public GTransformBuffer(int maxSize) {
            this.data = new LinkedList<GTransform>();
            this.maxSize = maxSize;
        }

        public void Enqueue(GTransform t) {
            if (data.Count >= maxSize) {
                if (circular) {
                    Dequeue();
                } else {
                    return;
                }
            }
            data.AddLast(t);
        }

        public void SetMaxSize(int maxSize) {
            this.maxSize = maxSize;
        }

        public void SetCircular(bool circular) {
            this.circular = circular;
        }

        public int Size() {
            return data.Count;
        }

        public void Dequeue() {
            data.RemoveFirst();
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
                if(counter >= maxSize) {
                    break;
                }
            }
            return ret;
        }

    }
}
