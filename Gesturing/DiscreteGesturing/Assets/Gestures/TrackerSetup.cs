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

        tracker.pathRenderer = lineRenderer;

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

    void Update () {
		
	}

    void GenerateGestures() {



        tracker.AddGesture("Ring-XZ", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(0, 0, 1), 90, new Vector3(0,0,0), .4f, ArcCheck.ARC_ORIENTATION.XZ),
            new ArcCheck(new Vector3(1, 0, 0), 90, new Vector3(0,0,0), .4f, ArcCheck.ARC_ORIENTATION.XZ),
            new ArcCheck(new Vector3(0, 0, -1), 90, new Vector3(0,0,0), .4f, ArcCheck.ARC_ORIENTATION.XZ),
            new ArcCheck(new Vector3(-1, 0, 0), 90, new Vector3(0,0,0), .4f, ArcCheck.ARC_ORIENTATION.XZ),
            },

          new CompositeNormalizer(new Vector3(-1, 0, -1), new Vector3(1, 0, 1)),
          new GestureEvent()));


        tracker.AddGesture("Ring-XY", new Gesture(new List<Check> {

             new ArcCheck(new Vector3(0, 1, 0), 90, new Vector3(0,0,0), .3f, ArcCheck.ARC_ORIENTATION.XY),
            new ArcCheck(new Vector3(1, 0, 0), 90, new Vector3(0,0,0), .3f, ArcCheck.ARC_ORIENTATION.XY),
            new ArcCheck(new Vector3(0, -1, 0), 90, new Vector3(0,0,0), .3f, ArcCheck.ARC_ORIENTATION.XY),
            new ArcCheck(new Vector3(-1, 0, 0), 90, new Vector3(0,0,0), .3f, ArcCheck.ARC_ORIENTATION.XY),

            },

          new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
          new GestureEvent()));


        tracker.AddGesture("Ring-YZ", new Gesture(new List<Check> {

             new ArcCheck(new Vector3(0, 1, 0), 90, new Vector3(0,0,0), .3f, ArcCheck.ARC_ORIENTATION.YZ),
            new ArcCheck(new Vector3(0, 0, 1), 90, new Vector3(0,0,0), .3f, ArcCheck.ARC_ORIENTATION.YZ),
            new ArcCheck(new Vector3(0, -1, 0), 90, new Vector3(0,0,0), .3f, ArcCheck.ARC_ORIENTATION.YZ),
            new ArcCheck(new Vector3(0, 0, -1), 90, new Vector3(0,0,0), .3f, ArcCheck.ARC_ORIENTATION.YZ),

            },

          new CompositeNormalizer(new Vector3(0, -1, -1), new Vector3(0, 1, 1)),
          new GestureEvent()));



        tracker.AddGesture("Fireball", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(0, 1, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(1, 0, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(0, -1, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(-1, 0, 0), 90, new Vector3(0,0,0)),

            new LineCheck(new Vector3(0,1, 0), new Vector3(0, -1, 0)),
            new RadiusCheck(new Vector3(0,0, 0)),

            },

          new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
          new GestureEvent()));




        tracker.AddGesture("Lightning",
        new Gesture(new List<Check> {
            new LineCheck(new Vector3(0,1, 0), new Vector3(-1, 0, 0)),
            new LineCheck(new Vector3(-1,0, 0), new Vector3(1, 0, 0)),
            new LineCheck(new Vector3(1,0, 0), new Vector3(0, -1, 0)),
         },

        new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
        new GestureEvent()));



        tracker.AddGesture("Parry",
         new Gesture(new List<Check> {
            new LineCheck(new Vector3(-1,1, 0), new Vector3(-1, -1, 0)),
            new LineCheck(new Vector3(-1,-1, 0), new Vector3(1, 1, 0)),

          },

         new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
         new GestureEvent()));


        float dp = 0.3f;
        tracker.AddGesture("Diamond",
          new Gesture(new List<Check> {
            new LineCheck(new Vector3(0,1, 0), new Vector3(1, 0, 0), dp),
            new LineCheck(new Vector3(1,0, 0), new Vector3(0, -1, 0), dp),
            new LineCheck(new Vector3(0,-1, 0), new Vector3(-1, 0, 0), dp),
            new LineCheck(new Vector3(-1,0, 0), new Vector3(0, 1, 0), dp),

            new LineCheck(new Vector3(0,1, 0), new Vector3(0, .5f, 0), dp),

            new LineCheck(new Vector3(0,.5f, 0), new Vector3(.5f, 0, 0), dp),
            new LineCheck(new Vector3(.5f,0, 0), new Vector3(0, -.5f, 0), dp),
            new LineCheck(new Vector3(0,-.5f, 0), new Vector3(-.5f, 0, 0), dp),
            new LineCheck(new Vector3(-.5f,0, 0), new Vector3(0, .5f, 0), dp),

           },

          new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
          new GestureEvent()));
          

        tracker.AddGesture("Heart",
           new Gesture(new List<Check> {
            new ArcCheck(new Vector3(0, .5f, 0), 90, new Vector3(.5f,.5f,0)),
            new ArcCheck(new Vector3(.5f, 1.0f, 0), 90, new Vector3(.5f,.5f,0)),
            new ArcCheck(new Vector3(-1, .5f, 0), 90, new Vector3(-.5f,.5f,0)),
            new ArcCheck(new Vector3(-.5f, 1.0f, 0), 90, new Vector3(-.5f,.5f,0)),
            new LineCheck(new Vector3(0, -1f, 0), new Vector3(.75f, -.25f, 0)),
            new LineCheck(new Vector3(0, -1f, 0), new Vector3(-.75f, -.25f, 0)),
            new LineCheck(new Vector3(.75f, -.25f, 0), new Vector3(1f, .5f, 0)),
            new LineCheck(new Vector3(-.75f, -.25f, 0), new Vector3(-1f, .5f, 0)),

            new RadiusCheck(new Vector3(0,.5f,0), .25f),
            },

           new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
           new GestureEvent()));


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


        tracker.AddGesture("Square", new SquareGesture());


        tracker.AddGesture("Letter-J", new Gesture(new List<Check> {
            new LineCheck(new Vector3(0, 1, 0), new Vector3(0, 0, 0)),
            new ArcCheck(new Vector3(0, 0, 0), 90, new Vector3(-.5f,0,0)),
            new ArcCheck(new Vector3(-.5f, -.5f, 0), 90, new Vector3(-.5f,0,0)),
            },

          new CompositeNormalizer(new Vector3(-1, -.5f, 0), new Vector3(0, 1, 0)),
          new GestureEvent()));


        tracker.AddGesture("2", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(.5f, .5f, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1, 0), -90, new Vector3(0,.5f,0)),
            new LineCheck(new Vector3(.5f, .5f, 0), new Vector3(-.5f, -1, 0)),
            new LineCheck(new Vector3(-.5f, -1, 0), new Vector3(.5f, -1, 0)),
            },

          new CompositeNormalizer(new Vector3(-.5f, -1, 0), new Vector3(.5f, 1, 0)),
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

         new CompositeNormalizer(new Vector3(-.5f, -1, 0), new Vector3(.5f, 1, 0)),
         new GestureEvent()));



        tracker.AddGesture("4", new Gesture(new List<Check> {
            new LineCheck(new Vector3(0, -1, 0), new Vector3(0, 1, 0)),
            new LineCheck(new Vector3(0, 1, 0), new Vector3(-.5f, 0, 0)),
            new LineCheck(new Vector3(-.5f, 0, 0), new Vector3(.5f, 0, 0)),
            },

         new CompositeNormalizer(new Vector3(-.5f, -1, 0), new Vector3(.5f, 1, 0)),
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
        string newText = "Gesture Detected: " + name + "\n" +
                         "Scale: " + data.scale.ToString("G4") + "\n" +
                         "Position: " + data.centroid.ToString("G4") + "\n" +
                         "Time: " + data.time.ToString("G4");
        text.SetText(newText);
    }

}
