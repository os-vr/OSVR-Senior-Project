using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gestures {

    /// <summary>
    /// Internal data storage class which can behave as either a set-size array or a circular array. 
    /// </summary>
    public class GTransformBuffer : IEnumerable<GTransform> {

        private int maxSize;
        private bool circular = false;
        private LinkedList<GTransform> data;

        /// <summary>
        /// Create a GTransformBuffer with a specified max size.
        /// </summary>
        /// <param name="maxSize">The desired max size of the GTransformBuffer</param>
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
        /// <summary>
        /// Sets the max size of the buffer.
        /// </summary>
        /// <param name="maxSize"></param>
        public void SetMaxSize(int maxSize) {
            this.maxSize = maxSize;
        }
        /// <summary>
        /// Sets the circular behavior according to the specified parameter.
        /// </summary>
        /// <param name="circular"></param>
        public void SetCircular(bool circular) {
            this.circular = circular;
        }

        /// <summary>
        /// Returns the number of elements in the buffer.
        /// </summary>
        /// <returns></returns>
        public int Size() {
            return data.Count;
        }

        /// <summary>
        /// Removes the first element of the buffer.
        /// </summary>
        public void Dequeue() {
            data.RemoveFirst();
        }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void Clear() {
            data.Clear();
        }

        /// <summary>
        /// Generates the Enumerator over the GTransforms in the buffer.
        /// </summary>
        /// <returns>IEnumerator<GTransform> of GTransform data points</returns>
        public IEnumerator<GTransform> GetEnumerator() {
            foreach (GTransform t in data) {
                yield return t;
            }
        }

        /// <summary>
        /// Returns the Enumerator over the GTransforrms in the buffer.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
         
        /// <summary>
        /// Returns the GTransformBuffer contents as a GTransform[] array;
        /// </summary>
        /// <returns>GTransform array  of the contents of the buffer.</returns>
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
