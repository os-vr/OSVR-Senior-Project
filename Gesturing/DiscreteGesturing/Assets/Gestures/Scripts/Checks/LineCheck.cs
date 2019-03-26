using UnityEditor;
using UnityEngine;

namespace Gestures {
    /// <summary>
    /// A Line check to check if 
    /// </summary>
    public class LineCheck : Check {

        public Vector3 firstPosition;
        public Vector3 secondPosition;
        public float precision;
        private LineRenderer lineRenderer;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstPosition">The first point of the line.</param>
        /// <param name="secondPosition">The second point of the line.</param>
        /// <param name="precision">Distance from the line considered to be on the line.</param>
        public LineCheck(Vector3 firstPosition, Vector3 secondPosition, float precision = 0.4f) {
            this.firstPosition = firstPosition;
            this.secondPosition = secondPosition;
            this.precision = precision;
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


        override public void VisualizeCheck(Rect grid) {
            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(Color.black);

            Vector2 seg = (secondPosition - firstPosition).normalized;
            Vector3 norm = new Vector3(seg.y, -seg.x);
            Vector3 p1 = firstPosition + precision/2 * norm;
            Vector3 p2 = firstPosition + -precision/2 * norm;
            Vector3 p3 = secondPosition + -precision/2 * norm;
            Vector3 p4 = secondPosition + precision /2 * norm;

            Vector3 size = new Vector3(grid.size.x, -grid.size.y, 0);

            GL.Vertex(Vector3.Scale(p1, size) + new Vector3(grid.position.x, grid.position.y));
            GL.Vertex(Vector3.Scale(p2, size) + new Vector3(grid.position.x, grid.position.y));
            GL.Vertex(Vector3.Scale(p3, size) + new Vector3(grid.position.x, grid.position.y));
            GL.Vertex(Vector3.Scale(p4, size) + new Vector3(grid.position.x, grid.position.y));
            GL.End();
            GL.PopMatrix();

            EditorGUILayout.BeginHorizontal();
            firstPosition = EditorGUILayout.Vector3Field("First Position: ", firstPosition);
            secondPosition = EditorGUILayout.Vector3Field("Second Position: ", secondPosition);
            EditorGUILayout.EndHorizontal();
        }

    }

}