using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Gestures {

    /// <summary>
    /// A Check to check if a point is within a certain radius of another point.
    /// </summary>
    public class RadiusCheck : Check {

        private Vector3 position;
        private float radius;

        /// <summary>
        /// Create a Radius Check used to determine whether a point fits inside the defined circular area. 
        /// </summary>
        /// <param name="position">The center of the circle</param>
        /// <param name="radius">A radius tolerance away from the center</param>
        public RadiusCheck(Vector3 position, float radius = 0.4f) {
            this.position = position;
            this.radius = radius;
        }

        /// <summary>
        /// Determine whether a single GTransform fits inside the area defined by the circle 
        /// </summary>
        /// <param name="g">GTransform to check position against</param>
        /// <returns> Returns a float (between 0 and 1) representing the distance from the center of the circle, or -1 if the check fails </returns>
        override public float CheckPasses(GTransform gTransform) {
            float distance = Vector3.Distance(gTransform.position, position);
            if (distance > radius) {
                return -1;
            }
            return 1;
        }

        public override void VisualizeCheck(Rect grid) {
            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetPass(0);
            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.blue);


            Vector3 p1 = position;
            Vector3 size = new Vector3(grid.size.x, -grid.size.y, 0);

            for(int i = 0; i < 9; i++) {
                p1 = position + radius * new Vector3(Mathf.Cos(2*3.14159f*i/8.0f), Mathf.Sin(2 * 3.14159f * i / 8.0f));
                GL.Vertex(Vector3.Scale(p1, size) + new Vector3(grid.position.x, grid.position.y));
            }
            GL.End();
            GL.PopMatrix();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.EndHorizontal();
        }


    }
}
