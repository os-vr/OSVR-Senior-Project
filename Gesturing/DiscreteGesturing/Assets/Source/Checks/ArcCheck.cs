using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    public class ArcCheck : Check {
        private GameObject line;
        private float precision;
        private Vector3 startPosition;
        private float degrees;
        private Vector3 center;
        private float radius;
        private const float eps = 0.01f;

        public ArcCheck(Vector3 startPosition, float degrees, Vector3 center, float precision = 0.4f) {
            this.startPosition = startPosition;
            this.degrees = degrees;
            this.center = center;
            this.radius = Vector3.Distance(center, startPosition);
            this.precision = precision;
            BuildGestureVisualization();

        }

        private bool isClockwise(Vector3 v1, Vector3 v2) {
            return Mathf.Sign(degrees) * (-v1.x * v2.y + v1.y * v2.x) >= 0;
        }

        public GStatus CheckPasses(GTransform g) {

            Vector3 position = g.position;
            Vector3 direction = (position - center).normalized;
            Vector3 sectorStart = (startPosition - center).normalized;
            Vector3 sectorEnd = (Quaternion.Euler(0, 0, -degrees) * sectorStart).normalized;
            float distance = Vector3.Distance(position, center);

            bool inArc = isClockwise(sectorStart, direction) && !isClockwise(sectorEnd, direction);
            bool prettyClose = (Vector3.Dot(sectorStart, direction) >= (1.0f - eps)) || (Vector3.Dot(sectorEnd, direction) >= (1.0f - eps));
            bool withinRadius = (distance < radius + precision/2.0f && distance > radius - precision/2.0f);

            if (withinRadius && (inArc || prettyClose)) {
                return GStatus.PASS;
            }
            return GStatus.FAIL;

        }


        private void BuildGestureVisualization() {
            line = new GameObject();
            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.material = Resources.Load<Material>("Transparent");

            Vector3[] arr = new Vector3[100];

            float startRadians = Mathf.Atan2(center.y - startPosition.y, center.x - startPosition.x) + Mathf.PI;
            float endRadians = startRadians - degrees * Mathf.PI / 180;
            float radianDifference = endRadians - startRadians;

            float r = radianDifference / 100.0f;
            for (int i = 0; i < 100; i++) {
                Vector3 pos = center + radius * new Vector3(Mathf.Cos(startRadians + r * i), Mathf.Sin(startRadians + r * i), 0) * (1.0f);
                arr[i] = pos;
            }

            lineRenderer.positionCount = 100;
            lineRenderer.SetPositions(arr);
            lineRenderer.widthMultiplier = precision;

            line.SetActive(false);

            line.transform.parent = Gesture.gestureVisualContainer.transform;
        }


        public void VisualizeCheck(bool active) {
            line.SetActive(active);
        }
    }
}
