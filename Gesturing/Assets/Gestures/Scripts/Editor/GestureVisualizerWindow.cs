using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Gestures {
    /// <summary>
    /// The EditorWindow for visualizing gestures.
    /// </summary>
    public class GestureVisualizerWindow : EditorWindow {
        private static GestureMonitor monitor;
        private static int selectedGesture = 0;
        private static Vector3 _boundsCenter = new Vector3(0, 0, 0);
        private static Vector3 _boundsSize = new Vector3(1, 1, 0);
        private static List<string> allGestures;
        private static BindingFlags flags = BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance;
        private static bool customBounds = false;

        [MenuItem("Tools/Gesture Visualization")]
        static void Init() {
            GestureVisualizerWindow window =
                (GestureVisualizerWindow)GetWindow(typeof(GestureVisualizerWindow));

            window.Show();
        }


        void DrawBoundingBox(Vector3 gridCenter, Vector3 gridScale, Vector3 boundsCenter, Vector3 boundsScale) {
            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex(gridCenter + Vector3.Scale(gridScale, new Vector3(1, 1, 0)) + new Vector3(1, 1, 0));
            GL.Vertex(gridCenter + Vector3.Scale(gridScale, new Vector3(-1, 1, 0)) + new Vector3(-1, 1, 0));
            GL.Vertex(gridCenter + Vector3.Scale(gridScale, new Vector3(-1, -1, 0)) + new Vector3(-1, -1, 0));
            GL.Vertex(gridCenter + Vector3.Scale(gridScale, new Vector3(1, -1, 0)) + new Vector3(1, -1, 0));
            GL.End();
            GL.PopMatrix();


            GL.PushMatrix();
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(Color.gray);
            GL.Vertex(gridCenter + Vector3.Scale(gridScale, new Vector3(1, 1, 0)));
            GL.Vertex(gridCenter + Vector3.Scale(gridScale, new Vector3(-1, 1, 0)));
            GL.Vertex(gridCenter + Vector3.Scale(gridScale, new Vector3(-1, -1, 0)));
            GL.Vertex(gridCenter + Vector3.Scale(gridScale, new Vector3(1, -1, 0)));
            GL.End();
            GL.PopMatrix();
        }

        void DrawMajorAxes(Vector3 gridCenter, Vector3 gridScale, Vector3 boundsCenter, Vector3 boundsScale) {
            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex(gridCenter + new Vector3(1, gridScale.y, 0));
            GL.Vertex(gridCenter + new Vector3(-0, gridScale.y, 0));
            GL.Vertex(gridCenter + new Vector3(-0, -gridScale.y, 0));
            GL.Vertex(gridCenter + new Vector3(1, -gridScale.y, 0));

            GL.Vertex(gridCenter + new Vector3(gridScale.x, 1, 0));
            GL.Vertex(gridCenter + new Vector3(gridScale.x, -0, 0));
            GL.Vertex(gridCenter + new Vector3(-gridScale.x, -0, 0));
            GL.Vertex(gridCenter + new Vector3(-gridScale.x, 1, 0));

            GL.End();
            GL.PopMatrix();

            string c_x = boundsCenter.x.ToString("G3");
            string c_y = boundsCenter.y.ToString("G3");
            GUI.Label(new Rect(gridCenter.x, gridCenter.y, 100, 20), "("+c_x+","+c_y+")");
        }

        void DrawTickMarks(Vector3 gridCenter, Vector3 gridScale, Vector3 boundsCenter, Vector3 boundsScale) {

            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(Color.black);

            GL.Vertex(gridCenter + new Vector3(1 + gridScale.x / 2, 10, 0));
            GL.Vertex(gridCenter + new Vector3(-0 + gridScale.x / 2, 10, 0));
            GL.Vertex(gridCenter + new Vector3(-0 + gridScale.x / 2, -10, 0));
            GL.Vertex(gridCenter + new Vector3(1 + gridScale.x / 2, -10, 0));

            GL.Vertex(gridCenter + new Vector3(1 - gridScale.x / 2, 10, 0));
            GL.Vertex(gridCenter + new Vector3(-0 - gridScale.x / 2, 10, 0));
            GL.Vertex(gridCenter + new Vector3(-0 - gridScale.x / 2, -10, 0));
            GL.Vertex(gridCenter + new Vector3(1 - gridScale.x / 2, -10, 0));


             GL.Vertex(gridCenter + new Vector3(10, 1 + gridScale.y / 2, 0));
            GL.Vertex(gridCenter + new Vector3(10, -0 + gridScale.y / 2, 0));
            GL.Vertex(gridCenter + new Vector3(-10, -0 + gridScale.y / 2, 0));
            GL.Vertex(gridCenter + new Vector3(-10, 1 + gridScale.y / 2, 0));

            GL.Vertex(gridCenter + new Vector3(10, 1 - gridScale.y / 2, 0));
            GL.Vertex(gridCenter + new Vector3(10, -0 - gridScale.y / 2, 0));
            GL.Vertex(gridCenter + new Vector3(-10, -0 - gridScale.y / 2, 0));
            GL.Vertex(gridCenter + new Vector3(-10, 1 - gridScale.y / 2, 0));

            GL.End();
            GL.PopMatrix();


            DrawLabel(new Vector3(gridCenter.x, gridCenter.y + gridScale.y / 2), new Vector3(boundsCenter.x, boundsCenter.y - 2*boundsScale.y / 2));
            DrawLabel(new Vector3(gridCenter.x, gridCenter.y - gridScale.y / 2), new Vector3(boundsCenter.x, boundsCenter.y + 2*boundsScale.y / 2));
            DrawLabel(new Vector3(gridCenter.x + gridScale.x / 2, gridCenter.y), new Vector3(boundsCenter.x + 2*boundsScale.x / 2, boundsCenter.y));
            DrawLabel(new Vector3(gridCenter.x - gridScale.x / 2, gridCenter.y), new Vector3(boundsCenter.x - 2*boundsScale.x / 2, boundsCenter.y));
        }

        void DrawLabel(Vector3 position, Vector3 bounds) {

            string c_x = bounds.x.ToString("G3");
            string c_y = bounds.y.ToString("G3");

            GUI.Label(new Rect(position.x, position.y, 100, 20), "(" + c_x + "," + c_y + ")");
        }


        void VisualizeLine(LineCheck c, Vector3 gridCenter, Vector3 gridScale, Vector3 boundsCenter, Vector3 boundsScale) {
            Vector3 firstPosition = (Vector3)typeof(LineCheck).GetField("firstPosition", flags).GetValue(c);
            Vector3 secondPosition = (Vector3)typeof(LineCheck).GetField("secondPosition", flags).GetValue(c);
            float tolerance = (float)typeof(LineCheck).GetField("tolerance", flags).GetValue(c);

            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(new Color(0, 0, 0, .25f));

            Vector2 seg = (secondPosition - firstPosition).normalized;
            Vector3 norm = new Vector3(seg.y, -seg.x);
            Vector3 p1 = firstPosition + tolerance / 2 * norm - boundsCenter;
            Vector3 p2 = firstPosition + -tolerance / 2 * norm - boundsCenter;
            Vector3 p3 = secondPosition + -tolerance / 2 * norm - boundsCenter;
            Vector3 p4 = secondPosition + tolerance / 2 * norm - boundsCenter;

            Vector3 size = new Vector3(gridScale.x / boundsScale.x / 2, -gridScale.y / boundsScale.y / 2, 0);

            GL.Vertex(Vector3.Scale(p1, size) + new Vector3(gridCenter.x, gridCenter.y));
            GL.Vertex(Vector3.Scale(p2, size) + new Vector3(gridCenter.x, gridCenter.y));
            GL.Vertex(Vector3.Scale(p3, size) + new Vector3(gridCenter.x, gridCenter.y));
            GL.Vertex(Vector3.Scale(p4, size) + new Vector3(gridCenter.x, gridCenter.y));
            GL.End();
            GL.PopMatrix();
        }

        void VisualizeRadius(RadiusCheck c, Vector3 gridCenter, Vector3 gridScale, Vector3 boundsCenter, Vector3 boundsScale) {
            Vector3 position = (Vector3)typeof(RadiusCheck).GetField("position", flags).GetValue(c);
            float radius = (float)typeof(RadiusCheck).GetField("radius", flags).GetValue(c);

            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetPass(0);
            GL.Begin(GL.LINE_STRIP);
            GL.Color(new Color(0, 0, 0, .25f));

            Vector3 p1 = position - boundsCenter;
            Vector3 size = new Vector3(gridScale.x / boundsScale.x / 2, -gridScale.y / boundsScale.y / 2, 0);

            for (int i = 0; i < 9; i++) {
                p1 = position + radius * new Vector3(Mathf.Cos(2 * 3.14159f * i / 8.0f), Mathf.Sin(2 * 3.14159f * i / 8.0f));
                GL.Vertex(Vector3.Scale(p1, size) + new Vector3(gridCenter.x, gridCenter.y));
            }
            GL.End();
            GL.PopMatrix();

        }

        void VisualizeArc(ArcCheck c, Vector3 gridCenter, Vector3 gridScale, Vector3 boundsCenter, Vector3 boundsScale) {
            Vector3 startPosition = (Vector3)typeof(ArcCheck).GetField("startPosition", flags).GetValue(c);
            Vector3 center = (Vector3)typeof(ArcCheck).GetField("center", flags).GetValue(c);
            float radius = (float)typeof(ArcCheck).GetField("radius", flags).GetValue(c);
            float degrees = (float)typeof(ArcCheck).GetField("degrees", flags).GetValue(c);
            float tolerance = (float)typeof(ArcCheck).GetField("tolerance", flags).GetValue(c);

            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetPass(0);
            GL.Begin(GL.TRIANGLE_STRIP);

            GL.Color(new Color(0, 0, 0, .25f));

            float startRadians = Mathf.Atan2(center.y - startPosition.y, center.x - startPosition.x) + Mathf.PI;
            float endRadians = startRadians - degrees * Mathf.PI / 180;
            float radianDifference = endRadians - startRadians;

            Vector3 size = new Vector3(gridScale.x / boundsScale.x / 2, -gridScale.y / boundsScale.y / 2, 0);

            int divisions = 16;
            float r = radianDifference / divisions;

            for (int i = 0; i <= divisions; i++) {
                Vector3 pos = center + radius * new Vector3(Mathf.Cos(startRadians + r * i), Mathf.Sin(startRadians + r * i), 0);

                Vector3 norm = (center - pos).normalized;
                GL.Vertex(Vector3.Scale(pos + tolerance / 2 * norm - boundsCenter, size) + new Vector3(gridCenter.x, gridCenter.y));
                GL.Vertex(Vector3.Scale(pos - tolerance / 2 * norm - boundsCenter, size) + new Vector3(gridCenter.x, gridCenter.y));
            }

            GL.End();
            GL.PopMatrix();
        }


        void OnGUI() {

            monitor = FindObjectOfType<GestureMonitor>();

            if (monitor == null) {
                return;
            }


            allGestures = new List<string>(monitor.GetGestureMap().Keys);
            selectedGesture = EditorGUILayout.Popup("Gesture", selectedGesture, allGestures.ToArray());
            _boundsCenter = EditorGUILayout.Vector3Field("Bounds Center", _boundsCenter);
            _boundsSize = EditorGUILayout.Vector3Field("Bounds Scale", _boundsSize);
            customBounds = EditorGUILayout.Toggle("Custom Bounds", customBounds);

            Dictionary<string, Gesture> dict = monitor.GetGestureMap();
            Gesture g = null;
            if (selectedGesture < allGestures.Count) {
                dict.TryGetValue(allGestures[selectedGesture], out g);
            }
            if (g == null) {
                return;
            }

            float width = this.position.width;
            float height = this.position.height;

            Vector3 gridCenter = new Vector3(width / 2, height / 2, 0);
            Vector3 gridScale = new Vector3(Mathf.Min(width, height) / 3, Mathf.Min(width, height) / 3, 0);
            Vector3 boundsCenter = _boundsCenter;
            Vector3 boundsScale = _boundsSize;

            FittedNormalizer normalizer = g.GetNormalizer() as FittedNormalizer;
            if(normalizer != null && ! customBounds) {
                Vector3 bl = (Vector3)typeof(FittedNormalizer).GetField("bottomLeft", flags).GetValue(normalizer);
                Vector3 tr = (Vector3)typeof(FittedNormalizer).GetField("topRight", flags).GetValue(normalizer);

                boundsCenter = (bl + tr) / 2.0f;
                boundsScale = (tr - bl) / 2.0f; 
            }

            DrawBoundingBox(gridCenter, gridScale, boundsCenter, boundsScale);
            DrawMajorAxes(gridCenter, gridScale, boundsCenter, boundsScale);
            DrawTickMarks(gridCenter, gridScale, boundsCenter, boundsScale);

            List<Check> checks = g.GetAllChecks();
            foreach (Check c in checks) {
                Type t = c.GetType();
                if (t == typeof(LineCheck)) {
                    VisualizeLine((LineCheck)c, gridCenter, gridScale, boundsCenter, boundsScale);
                } else if (t == typeof(ArcCheck)) {
                    VisualizeArc((ArcCheck)c, gridCenter, gridScale, boundsCenter, boundsScale);
                } else if (t == typeof(RadiusCheck)) {
                    VisualizeRadius((RadiusCheck)c, gridCenter, gridScale, boundsCenter, boundsScale);
                }
            }

        }


    }
}