using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures {
    public class RadiusCheck : Check {
        private Vector3 position;
        private float radius;
        private GameObject sphere;


        public RadiusCheck(Vector3 position, float radius = 0.4f) {
            this.position = position;
            this.radius = radius;

            BuildGestureVisualization();
        }

        public GStatus CheckPasses(GTransform gTransform) {
            float distance = Vector3.Distance(gTransform.position, position);
            if (distance > radius) {
                return GStatus.FAIL;
            }

            return GStatus.PASS;
        }


        public bool CheckAll(List<GTransform> transforms) {
            return true;
        }

        private void BuildGestureVisualization() {
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<Renderer>().material = Resources.Load<Material>("Transparent");
            sphere.transform.position = position;
            sphere.transform.localScale = new Vector3(radius, radius, radius);
            sphere.SetActive(false);

            sphere.transform.parent = Gesture.gestureVisualContainer.transform;
        }

        public void VisualizeCheck(bool active) {
            sphere.SetActive(active);

        }
    }
}
