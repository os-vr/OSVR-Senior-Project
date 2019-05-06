using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gestures;
using UnityEngine.Events;


public class TrackerSetup : MonoBehaviour {

    public TextMesh text;
    private GestureMonitor tracker;
    public LineRenderer lineRenderer;
    public IController controller;

    void Start () {
        tracker = gameObject.AddComponent<GestureMonitor>();
        tracker.controller = controller;
        tracker.lineRenderer = lineRenderer;

        GenerateGestures();

        tracker.AddGestureCompleteCallback(GestureComplete);
        tracker.AddGestureFailedCallback(GestureFailed);
        tracker.AddGestureStartCallback(GestureStart);

    }

    
    void GestureStart() {
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
    }

	
    void GestureComplete(GestureMetaData data) {
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        SetText(data);
    }


    void GestureFailed(GestureMetaData data) {
        string newText = "Result: <i><color=red>" + "None" + "</color></i>";
        text.text = newText;
    }


    void GenerateGestures() {

        tracker.AddGesture("Square", new SquareGesture(.6f));
        tracker.AddGesture("Circle", new CircleGesture(.4f));
        tracker.AddGesture("Triangle", new TriangleGesture(.8f));
        tracker.AddGesture("Heart", new HeartGesture());

        tracker.AddGesture("Letter-S", new Gesture().AddChecks(new List<Check> {
            new ArcCheck(new Vector3(.5f, .5f, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(-.5f,.5f,0), -90, new Vector3(0,.5f,0)),

            new ArcCheck(new Vector3(0, 0, 0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(.5f,-.5f,0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(0,-1,0), 90, new Vector3(0,-.5f,0)) })
            
            .SetNormalizer(new FittedNormalizer(new Vector3(-.5f, -1.0f, 0), new Vector3(.5f, 1.0f, 0))));


        tracker.AddGesture("Plus", new Gesture().AddChecks(new List<Check>() {
            new LineCheck(new Vector3(-1,0,0), new Vector3(1,0,0)),
            new LineCheck(new Vector3(0,-1,0), new Vector3(0,1,0)),

            new RadiusCheck(new Vector3(-1,0,0)),
            new RadiusCheck(new Vector3(1,0,0)),
            new RadiusCheck(new Vector3(0,-1,0)),
            new RadiusCheck(new Vector3(0,1,0)),

        }).SetNormalizer(new FittedNormalizer()));

    }



    private void SetText(GestureMetaData data) {
        string newText = "Result: <i><color=green>" + data.name + "</color></i>";
        text.text = newText;
    }

}
