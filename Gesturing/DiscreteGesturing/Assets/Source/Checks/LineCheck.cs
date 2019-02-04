using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gestures {
    public class LineCheck : Check {
        private Vector3 firstPosition;
        private Vector3 secondPosition;
        private GameObject line;
        private float precision;

        public LineCheck(Vector3 firstPosition, Vector3 secondPosition, float precision = 0.4f) {
            this.firstPosition = firstPosition;
            this.secondPosition = secondPosition;
            this.precision = precision;

            BuildGestureVisualization();
        }


        public GStatus CheckPasses(GTransform g) {

            float distance = Vector3.Distance(g.position, GetClosestPointOnLineSegment(firstPosition, secondPosition, g.position));
            if (distance > precision/2.0f) {
                return GStatus.FAIL;
            }

            return GStatus.PASS;
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
            line = new GameObject();

            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.material = Resources.Load<Material>("Transparent");
            lineRenderer.SetPositions(new Vector3[] { firstPosition, secondPosition });
            lineRenderer.widthMultiplier = precision;
            line.SetActive(false);

            line.transform.parent = Gesture.gestureVisualContainer.transform;
        }

        public void VisualizeCheck(bool active) {
            line.SetActive(active);
        }


    }
}