using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*

namespace Gestures {
    [CustomEditor(typeof(GestureMonitor), true)]
    public class GestureMonitorEditor : Editor {

        private GestureMonitor monitor;

        public void OnEnable() {
            monitor = (GestureMonitor)serializedObject.targetObject;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            

            EditorGUILayout.ObjectField(serializedObject.FindProperty("controller"));
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Hello World");
            EditorGUILayout.Separator();
            DrawList(serializedObject.FindProperty("list"), monitor.list);
            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }

        public void DrawList(SerializedProperty prop, List<LineCheck> list) {
            
           
            int newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("size", list.Count));
            while (newCount < list.Count) {
                //list[list.Count - 1].Remove();
                list.RemoveAt(list.Count - 1);
            }
            while (newCount > list.Count)
                list.Add(new LineCheck(new Vector3(0,0,0), new Vector3(0,0,0)));

            for (int i = 0; i < prop.arraySize; i++) {
                SerializedProperty pr = prop.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(pr, new GUIContent("Test"), true);
            }
            

        }

    }
}
*/