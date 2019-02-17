using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gestures {

    public class RadiusCheck : Check {
        private Vector3 position;
        private float radius;


        public RadiusCheck(Vector3 position, float radius = 0.4f) {
            this.position = position;
            this.radius = radius;

            BuildGestureVisualization();
        }

        override public float CheckPasses(GTransform gTransform) {
            float distance = Vector3.Distance(gTransform.position, position);
            if (distance > radius) {
                return -1;
            }
            return 1;
        }


        public bool CheckAll(List<GTransform> transforms) {
            return true;
        }

        private void BuildGestureVisualization() {
            visualizationObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visualizationObject.GetComponent<Renderer>().material = Resources.Load<Material>("Transparent");
            visualizationObject.transform.position = position;
            visualizationObject.transform.localScale = new Vector3(radius, radius, radius);
            visualizationObject.SetActive(false);

            visualizationObject.transform.parent = Gesture.gestureVisualContainer.transform;
        }


    }
}
