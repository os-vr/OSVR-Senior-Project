using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gestures {
    public class GestureVisualizerWindow : EditorWindow {
        private static GestureMonitor monitor;
        private static int selectedGesture = 0;
        private static List<string> allGestures;

        [MenuItem("Window/Gesture Visualization")]
        static void Init() {
            GestureVisualizerWindow window =
                (GestureVisualizerWindow)GetWindow(typeof(GestureVisualizerWindow));

            window.Show();
        }

        void OnGUI() {
            if(monitor == null) {
                monitor = FindObjectOfType<GestureMonitor>().GetComponent<GestureMonitor>();
                allGestures = new List<string>(monitor.GetGestureMap().Keys);
            }

            selectedGesture = EditorGUILayout.Popup("Gesture", selectedGesture, allGestures.ToArray());

            float width = this.position.width;
            float height = this.position.height;

            Vector3 gridCenter = new Vector3(width / 2, height / 2, 0);
            Vector3 gridSize = new Vector3(width / 4, width / 4, 0);

            Rect grid = new Rect(gridCenter.x, gridCenter.y, 
                                gridSize.x, gridSize.y);

            GL.PushMatrix();
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(Color.gray);
            GL.Vertex(gridCenter + Vector3.Scale(gridSize, new Vector3(1, 1, 0)));
            GL.Vertex(gridCenter + Vector3.Scale(gridSize, new Vector3(-1, 1, 0)));
            GL.Vertex(gridCenter + Vector3.Scale(gridSize, new Vector3(-1, -1, 0)));
            GL.Vertex(gridCenter + Vector3.Scale(gridSize, new Vector3(1, -1, 0)));
            GL.End();
            GL.PopMatrix();

            /*
            Handles.color = Color.red;
            Handles.DrawLine(new Vector3(0, 0, 0), new Vector3(200,200,0));
            Handles.DrawSolidDisc(new Vector3(width / 2, height / 2, 0), new Vector3(0, 0, 1), 50);
            //Handles.Draw
            */

            if (monitor != null) {
                Dictionary<string, Gesture> dict = monitor.GetGestureMap();
                Gesture g;
                dict.TryGetValue(allGestures[selectedGesture], out g);
                if (g != null) {
                    g.VisualizeGesture(grid);
                }
            }
            
        }
    }
}