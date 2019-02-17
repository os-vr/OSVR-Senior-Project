using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gestures {

    [ExecuteInEditMode]
    [System.Serializable]
    public class LineCheck : Check {

        public Vector3 firstPosition;
        public Vector3 secondPosition;
        public float precision;
        private LineRenderer lineRenderer;

        public LineCheck(Vector3 firstPosition, Vector3 secondPosition, float precision = 0.4f) {
            this.firstPosition = firstPosition;
            this.secondPosition = secondPosition;
            this.precision = precision;

            BuildGestureVisualization();
        }

        override public float CheckPasses(GTransform g) {

            float distance = Vector3.Distance(g.position, GetClosestPointOnLineSegment(firstPosition, secondPosition, g.position));
            if (distance > precision/2.0f) {
                return -1;
            }

            return distance/(precision/2.0f);
        }


        public Vector3 GetClosestPointOnLineSegment(Vector3 A, Vector3 B, Vector3 P) {
            Vector3 AP = P - A;
            Vector3 AB = B - A;

            float magnitudeAB = AB.sqrMagnitude;
            float ABAPproduct = Vector3.Dot(AP, AB);
            float distance = ABAPproduct / magnitudeAB;

            if (distance < 0) {
                return A;

            } else if (distance > 1) {
                return B;
            } else {
                return A + AB * distance;
            }
        }

        private void BuildGestureVisualization() {
            visualizationObject = new GameObject();

            lineRenderer = visualizationObject.AddComponent<LineRenderer>();
            lineRenderer.material = Resources.Load<Material>("Transparent");
            lineRenderer.SetPositions(new Vector3[] { firstPosition, secondPosition });
            lineRenderer.widthMultiplier = precision;
            visualizationObject.SetActive(false);

            visualizationObject.transform.parent = Gesture.GetVisualContainerTransform();
        }

        public void Validate() {
            lineRenderer.SetPositions(new Vector3[] { firstPosition, secondPosition });
            lineRenderer.widthMultiplier = precision;
        }
    }

}