using UnityEditor;
using UnityEngine;


namespace Gestures {
    /// <summary>
    /// A Check to determine whether a point fits inside a line segment.
    /// </summary>
    public class LineCheck : Check {

        private Vector3 firstPosition;
        private Vector3 secondPosition;
        private float precision;
        private LineRenderer lineRenderer;

        /// <summary>
        /// Create a Line Check used to determine whether a point fits inside the defined line segment. 
        /// </summary>
        /// <param name="firstPosition">The first point of the line segment.</param>
        /// <param name="secondPosition">The second point of the line segment.</param>
        /// <param name="tolerance">Distance from the line considered to be on the line.</param>
        public LineCheck(Vector3 firstPosition, Vector3 secondPosition, float tolerance = 0.4f) {
            this.firstPosition = firstPosition;
            this.secondPosition = secondPosition;
            this.precision = tolerance;
        }


        /// <summary>
        /// Determine whether a single GTransform fits inside the area defined by the line segment 
        /// </summary>
        /// <param name="g">GTransform to check position against</param>
        /// <returns> Returns a float (between 0 and 1) representing the distance from the center of the line, or -1 if the check fails </returns>
        override public float CheckPasses(GTransform g) {
            float distance = Vector3.Distance(g.position, GetClosestPointOnLineSegment(firstPosition, secondPosition, g.position));
            if (distance > precision/2.0f) {
                return -1;
            }

            return distance/(precision/2.0f);
        }


        private Vector3 GetClosestPointOnLineSegment(Vector3 A, Vector3 B, Vector3 P) {
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