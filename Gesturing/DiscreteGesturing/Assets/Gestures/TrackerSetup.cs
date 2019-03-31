using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gestures;
using UnityEngine.Events;
using TMPro;

public class TrackerSetup : MonoBehaviour {

    public TextMeshPro text;
    private GestureMonitor tracker;
    public LineRenderer lineRenderer;

    void Start () {
        tracker = gameObject.AddComponent<GestureMonitor>();
        tracker.controller = GetComponentInChildren<IController>();

        tracker.lineRenderer = lineRenderer;

        GenerateGestures();

        tracker.AddGestureCompleteCallback(GestureComplete);
        tracker.AddGestureFailedCallback(GestureFailed);
        tracker.AddGestureStartCallback(GestureStart);

        tracker.SetTrackAllGestures(true);
        tracker.SetTrackGesture(new List<string>() { "Circle", "Triangle", "Horizontal" }, true);

    }

    
    void GestureStart() {
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
    }

	
    void GestureComplete(GestureMetaData data) {
        if (!data.name.Equals("Question")) {
            SetText(data.name, data);
        }
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        if (data.name.Equals("Circle")) {
            tracker.SetMaxBufferSize(128);
            tracker.SetBufferWrap(true);
        }

        if (data.name.Equals("Horizontal")) {
            tracker.SetMaxBufferSize(512);
            tracker.SetBufferWrap(false);
        }

        if (data.name.Equals("Square")) {
            lineRenderer.startColor = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
            lineRenderer.endColor = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
        }

    }


    void GestureFailed(GestureMetaData data) {
        text.SetText("---");
    }


    void GenerateGestures() {

        tracker.AddGesture("Square", new SquareGesture());
        tracker.AddGesture("Circle", new CircleGesture());
        tracker.AddGesture("Triangle", new TriangleGesture());
        tracker.AddGesture("Heart", new HeartGesture());

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


        tracker.AddGesture("Letter-J", new Gesture(new List<Check> {
            new LineCheck(new Vector3(0, 1, 0), new Vector3(0, 0, 0)),
            new ArcCheck(new Vector3(0, 0, 0), 90, new Vector3(-.5f,0,0)),
            new ArcCheck(new Vector3(-.5f, -.5f, 0), 90, new Vector3(-.5f,0,0)),
            },

          new FittedNormalizer(new Vector3(-1, -.5f, 0), new Vector3(0, 1, 0)),
          new GestureEvent()));


        tracker.AddGesture("2", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(.5f, .5f, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1, 0), -90, new Vector3(0,.5f,0)),
            new LineCheck(new Vector3(.5f, .5f, 0), new Vector3(-.5f, -1, 0)),
            new LineCheck(new Vector3(-.5f, -1, 0), new Vector3(.5f, -1, 0)),
            },

          new FittedNormalizer(new Vector3(-.5f, -1, 0), new Vector3(.5f, 1, 0)),
          new GestureEvent()));


        tracker.AddGesture("3", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(.5f, .5f, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(.5f, .5f, 0), 90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1, 0), -90, new Vector3(0,.5f,0)),

            new ArcCheck(new Vector3(.5f, -.5f, 0), -90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(.5f, -.5f, 0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(0, -1, 0), 90, new Vector3(0,-.5f,0)),

            new LineCheck(new Vector3(0, 0, 0), new Vector3(-.5f, 0, 0)),

            },

         new FittedNormalizer(new Vector3(-.5f, -1, 0), new Vector3(.5f, 1, 0)),
         new GestureEvent()));



        tracker.AddGesture("4", new Gesture(new List<Check> {
            new LineCheck(new Vector3(0, -1, 0), new Vector3(0, 1, 0)),
            new LineCheck(new Vector3(0, 1, 0), new Vector3(-.5f, 0, 0)),
            new LineCheck(new Vector3(-.5f, 0, 0), new Vector3(.5f, 0, 0)),
            },

         new FittedNormalizer(new Vector3(-.5f, -1, 0), new Vector3(.5f, 1, 0)),
         new GestureEvent()));


        tracker.AddGesture("Letter-S", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(.5f, .5f, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(-.5f,.5f,0), -90, new Vector3(0,.5f,0)),

            new ArcCheck(new Vector3(0, 0, 0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(.5f,-.5f,0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(0,-1,0), 90, new Vector3(0,-.5f,0)),
            },

          new FittedNormalizer(new Vector3(-.5f, -1.0f, 0), new Vector3(.5f, 1.0f, 0)),
          new GestureEvent()));


        tracker.AddGesture("Letter-P", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(0, 1, 0), 90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(.5f,.5f,0), 90, new Vector3(0,.5f,0)),

            new LineCheck(new Vector3(0,1,0), new Vector3(0,-1,0)),


            },

          new FittedNormalizer(new Vector3(0, -1.0f, 0), new Vector3(0.5f, 1.0f, 0)),
          new GestureEvent()).AddSequentialChecks(
            new List<Check>{
                new RadiusCheck(new Vector3(0,-1,0)),
                new RadiusCheck(new Vector3(0,1,0)),
                new RadiusCheck(new Vector3(.5f,.5f,0)),
                new RadiusCheck(new Vector3(0,0,0)),
            }
            ));



        tracker.AddGesture("Letter-Z", new Gesture(new List<Check> {
            new LineCheck(new Vector3(-1, 1, 0), new Vector3(1, 1, 0)),
            new LineCheck(new Vector3(1, 1, 0), new Vector3(-1, -1, 0)),
            new LineCheck(new Vector3(-1, -1, 0), new Vector3(1, -1, 0)) },

           new FittedNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
           new GestureEvent()));



        tracker.AddGesture("Question", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(-.5f, .5f, 0), 90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1.0f, 0), 90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(.5f, .5f, 0), 90, new Vector3(0,.5f,0)),

            new LineCheck(new Vector3(0, 0, 0),new Vector3(0, -1, 0))},

          new FittedNormalizer(new Vector3(-.5f, -1, 0), new Vector3(.5f, 1, 0)),
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
        string newText = "Gesture Detected: " + name + "\n" +
                         "Scale: " + data.scale.ToString("G4") + "\n" +
                         "Position: " + data.centroid.ToString("G4") + "\n" +
                         "Speed: " + data.averageSpeed.ToString("G4");
        text.SetText(newText);
    }

}
