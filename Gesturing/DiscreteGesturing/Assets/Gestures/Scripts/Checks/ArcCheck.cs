using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gestures {
    /// <summary>
    /// An Arc check in a single plane.
    /// </summary>
    public class ArcCheck : Check {

        public enum ARC_ORIENTATION {
            XY,
            YZ,
            XZ,
        };

        private float precision;
        private Vector3 startPosition;
        private float degrees;
        private Vector3 center;
        private float radius;
        private const float eps = 0.01f;
        private ARC_ORIENTATION orientation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPosition">The starting position of the arc.</param>
        /// <param name="degrees">Number of degrees to extent. MUST be between -90 and 90 degrees, otherwise unpredictable consequences occur.</param>
        /// <param name="center">The center point of circle on which the Arc lies.</param>
        /// <param name="precision">The distance tolerance from which a point can be considered within the arc.</param>
        /// <param name="orientation">ARC_ORIENTATION enum, specifying which plane the arc resides in</param>
        public ArcCheck(Vector3 startPosition, float degrees, Vector3 center, float precision = 0.4f, ARC_ORIENTATION orientation = ARC_ORIENTATION.XY) {
            this.startPosition = startPosition;
            this.degrees = degrees;
            this.center = center;
            this.radius = Vector3.Distance(center, startPosition);
            this.precision = precision;
            this.orientation = orientation;
        }

        private bool IsClockwise(Vector3 v1, Vector3 v2) {
            if (orientation == ARC_ORIENTATION.XY) {
                return Mathf.Sign(degrees) * (-v1.x * v2.y + v1.y * v2.x) >= 0;
            }

            if (orientation == ARC_ORIENTATION.XZ) {
                return Mathf.Sign(degrees) * (-v1.z * v2.x + v1.x * v2.z) >= 0;
            }

            return Mathf.Sign(degrees) * (-v1.y * v2.z + v1.z * v2.y) >= 0;

        }

        override public float CheckPasses(GTransform g) {

            Vector3 rotation = new Vector3(
                (orientation == ARC_ORIENTATION.YZ) ? 1 : 0, 
                (orientation == ARC_ORIENTATION.XZ) ? 1 : 0, 
                (orientation == ARC_ORIENTATION.XY) ? 1 : 0);
            Quaternion qrot = Quaternion.Euler(-degrees*rotation);

            Vector3 position = g.position;
            Vector3 direction = (position - center).normalized;
            Vector3 sectorStart = (startPosition - center).normalized;
            Vector3 sectorEnd = (qrot * sectorStart).normalized;
            float distance = Vector3.Distance(position, center);

            bool inArc = IsClockwise(sectorStart, direction) && !IsClockwise(sectorEnd, direction);
            bool prettyClose = (Vector3.Dot(sectorStart, direction) >= (1.0f - eps)) || (Vector3.Dot(sectorEnd, direction) >= (1.0f - eps));
            bool withinRadius = (distance < radius + precision/2.0f && distance > radius - precision/2.0f);

            if (withinRadius && (inArc || prettyClose)) {
                return Mathf.Abs((distance - radius))/(precision/2.0f);
            }
            return -1;

        }

        override public void VisualizeCheck(Rect grid) {
            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetPass(0);
            GL.Begin(GL.TRIANGLE_STRIP);
           
            GL.Color(Color.black);

            float startRadians = Mathf.Atan2(center.y - startPosition.y, center.x - startPosition.x) + Mathf.PI;
            float endRadians = startRadians - degrees * Mathf.PI / 180;
            float radianDifference = endRadians - startRadians;

            Vector3 size = new Vector3(grid.size.x, -grid.size.y, 0);

            int divisions = 16;
            float r = radianDifference / divisions;

            for (int i = 0; i <= divisions; i++) {
                Vector3 pos = center + radius * new Vector3(Mathf.Cos(startRadians + r * i), Mathf.Sin(startRadians + r * i), 0);
               
                Vector3 norm = (center - pos).normalized;
                GL.Vertex(Vector3.Scale(pos + precision / 2 * norm, size) + new Vector3(grid.position.x, grid.position.y));
                GL.Vertex(Vector3.Scale(pos - precision / 2 * norm, size) + new Vector3(grid.position.x, grid.position.y));
            }

            GL.End();
            GL.PopMatrix();



            EditorGUILayout.BeginHorizontal();
            startPosition = EditorGUILayout.Vector3Field("Start : ", startPosition);
            center = EditorGUILayout.Vector3Field("Center : ", center);
            degrees = EditorGUILayout.Slider("Degrees : ", degrees, -90, 90);
            EditorGUILayout.EndHorizontal();

        }

    }
}
