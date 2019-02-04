using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gestures;
using UnityEngine.Events;
using TMPro;

public class TrackerSetup : MonoBehaviour {

    public TextMeshPro text;
    private GestureMonitor tracker;

    void Start () {
        tracker = gameObject.AddComponent<GestureMonitor>();
        tracker.controller = GetComponentInChildren<IController>();

        GenerateGestures();

        tracker.AddGestureCompleteCallback(GestureComplete);
        tracker.AddGestureFailedCallback(GestureFailed);
        tracker.SetTrackAllGestures(false);
        tracker.SetTrackGesture(new List<string>() { "Circle", "Triangle", "Question" }, true);

        DefaultGestureLineRenderer pathRenderer = gameObject.AddComponent<DefaultGestureLineRenderer>();
        tracker.pathRenderer = pathRenderer.lineRenderer;
    }
	
    void GestureComplete(GestureMetaData data) {
        SetText(data.gestureName, data);
    }

    void GestureFailed(GestureMetaData data) {
        
    }

    void Update () {
		
	}

    void GenerateGestures() {

        tracker.AddGesture("Vertical", 
           new Gesture(new List<Check> {
               new LineCheck(new Vector3(0, 1, 0), new Vector3(0, -1, 0)) },
               new LineNormalizer(),
               new GestureEvent()));


        tracker.AddGesture("Horizontal",
           new Gesture(new List<Check> {
               new LineCheck(new Vector3(-1, 0, 0), new Vector3(1, 0, 0)) },
               new LineNormalizer(),
               new GestureEvent()));


        tracker.AddGesture("Square", new Gesture(new List<Check> {
                new LineCheck(new Vector3(1, 1, 0), new Vector3(-1, 1, 0)),
                new LineCheck(new Vector3(-1, 1, 0), new Vector3(-1, -1, 0)),
                new LineCheck(new Vector3(-1, -1, 0), new Vector3(1, -1, 0)),
                new LineCheck(new Vector3(1, -1, 0), new Vector3(1, 1, 0)),

                new RadiusCheck(new Vector3(1, 1, 0)),
                new RadiusCheck(new Vector3(-1, 1, 0)),
                new RadiusCheck(new Vector3(-1, -1, 0)),
                new RadiusCheck(new Vector3(1, -1, 0)),

            },


            new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
            new GestureEvent()));



        tracker.AddGesture("Letter-J", new Gesture(new List<Check> {
            new LineCheck(new Vector3(0, 1, 0), new Vector3(0, 0, 0)),
            new ArcCheck(new Vector3(0, 0, 0), 90, new Vector3(-.5f,0,0)),
            new ArcCheck(new Vector3(-.5f, -.5f, 0), 90, new Vector3(-.5f,0,0)),
            },

          new CompositeNormalizer(new Vector3(-1, -.5f, 0), new Vector3(0, 1, 0)),
          new GestureEvent()));


        tracker.AddGesture("Letter-S", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(.5f, .5f, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(-.5f,.5f,0), -90, new Vector3(0,.5f,0)),

            new ArcCheck(new Vector3(0, 0, 0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(.5f,-.5f,0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(0,-1,0), 90, new Vector3(0,-.5f,0)),
            },

          new CompositeNormalizer(new Vector3(-.5f, -1.0f, 0), new Vector3(.5f, 1.0f, 0)),
          new GestureEvent()));


        tracker.AddGesture("Letter-P", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(0, 1, 0), 90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(.5f,.5f,0), 90, new Vector3(0,.5f,0)),

            new LineCheck(new Vector3(0,1,0), new Vector3(0,-1,0)),


            },

            new List<Check>{
                new RadiusCheck(new Vector3(0,-1,0)),
                new RadiusCheck(new Vector3(0,1,0)),
                new RadiusCheck(new Vector3(.5f,.5f,0)),
                new RadiusCheck(new Vector3(0,0,0)),
            },

          new CompositeNormalizer(new Vector3(0, -1.0f, 0), new Vector3(0.5f, 1.0f, 0)),
          new GestureEvent()));



        tracker.AddGesture("Letter-Z", new Gesture(new List<Check> {
            new LineCheck(new Vector3(-1, 1, 0), new Vector3(1, 1, 0)),
            new LineCheck(new Vector3(1, 1, 0), new Vector3(-1, -1, 0)),
            new LineCheck(new Vector3(-1, -1, 0), new Vector3(1, -1, 0)) },

           new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
           new GestureEvent()));


        tracker.AddGesture("Triangle", new Gesture(new List<Check> {
            new LineCheck(new Vector3(-1, -1, 0), new Vector3(0, 1.0f, 0)),
            new LineCheck(new Vector3(0, 1.0f, 0), new Vector3(1, -1, 0)),
            new LineCheck(new Vector3(1, -1, 0),new Vector3(-1, -1, 0))},

            new CompositeNormalizer(),
            new GestureEvent()));


        tracker.AddGesture("Circle", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(0, 1, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(1, 0, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(0, -1, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(-1, 0, 0), 90, new Vector3(0,0,0)),
            },

           new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
           new GestureEvent()));




        tracker.AddGesture("Question", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(-.5f, .5f, 0), 90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1.0f, 0), 90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(.5f, .5f, 0), 90, new Vector3(0,.5f,0)),

            new LineCheck(new Vector3(0, 0, 0),new Vector3(0, -1, 0))},

          new CompositeNormalizer(new Vector3(-.5f, -1, 0), new Vector3(.5f, 1, 0)),
          new GestureEvent(DisplayHints)));


    }


    private void DisplayHints(GestureMetaData data) {
        string newText = "Available Gestures: ";
        List<string> keys = new List<string>(tracker.GetGestureMap().Keys);
        for (int i = 0; i < keys.Count; i++) {
            if (i % 5 == 0) {
                newText += "\n";
            }
            newText += keys[i] + ", ";
        }
        text.SetText(newText);
    }


    private void SetText(string name, GestureMetaData data) {
        string newText = "Gesture Name: " + name + "\n" +
                         "Scale: " + data.range.ToString("G4") + "\n" +
                         "Position: " + data.gestureCenter.ToString("G4");
        text.SetText(newText);
    }

}
