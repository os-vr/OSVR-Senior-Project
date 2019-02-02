using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GestureMonitor : MonoBehaviour {
    public GestureRenderer pathRenderer;
    public IController controller;
    public TextMeshPro text;

    private Normalizer viewNormalizer;
    private Dictionary<string, Gesture> gestureMap = new Dictionary<string, Gesture>();
    public List<string> gestureNames;
    private GTransformBuffer dataQueue;
    private bool gestureActivePreviousFlag = false;

    private Gesture displayGesture = null;
    public String displayGestureString = "";
    public bool displayGestureVisible = true;
    public bool renderDebugPath = true;

    public UnityEvent<string> gestureObservedCallback;

    private GestureMetaData metaData;

    void GenerateGestures()
    {
        Gesture.gestureVisualContainer = new GameObject();
        Gesture.gestureVisualContainer.name = "Gesture Visual Container";

        gestureMap["Vertical"] =
           new Gesture(new List<Check> {
               new LineCheck(new Vector3(0, 1, 0), new Vector3(0, -1, 0)) }, 
               new LineNormalizer(), 
               new GestureEvent(delegate(GestureMetaData data) { SetText("Vertical Line", data); }));


        gestureMap["Horizontal"] =
           new Gesture(new List<Check> {
               new LineCheck(new Vector3(-1, 0, 0), new Vector3(1, 0, 0)) },
               new LineNormalizer(),
               new GestureEvent(delegate (GestureMetaData data) { SetText("Horizontal Line", data); }));


        gestureMap["Square"] = new Gesture(new List<Check> {
            new LineCheck(new Vector3(1, 1, 0), new Vector3(-1, 1, 0)),
            new LineCheck(new Vector3(-1, 1, 0), new Vector3(-1, -1, 0)),
            new LineCheck(new Vector3(-1, -1, 0), new Vector3(1, -1, 0)),
            new LineCheck(new Vector3(1, -1, 0), new Vector3(1, 1, 0))},

            new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
            new GestureEvent(delegate (GestureMetaData data) { SetText("Square", data); }));

         
        float precision = 0.25f;
        gestureMap["Letter-B"] = new Gesture(new List<Check> {
            new LineCheck(new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), precision),
            new LineCheck(new Vector3(-1, 1, 0), new Vector3(0, .5f, 0), precision),
            new LineCheck(new Vector3(0, .5f, 0), new Vector3(-1, 0, 0), precision),
            new LineCheck(new Vector3(-1, 0, 0), new Vector3(0, -.5f, 0), precision),
            new LineCheck(new Vector3(0, -0.5f, 0), new Vector3(-1, -1, 0), precision)},

           new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(0, 1, 0)),
           new GestureEvent(delegate (GestureMetaData data) { SetText("B", data); }));


        gestureMap["Letter-J"] = new Gesture(new List<Check> {
            new LineCheck(new Vector3(0, 1, 0), new Vector3(0, 0, 0)),
            new ArcCheck(new Vector3(0, 0, 0), 90, new Vector3(-.5f,0,0)),
            new ArcCheck(new Vector3(-.5f, -.5f, 0), 90, new Vector3(-.5f,0,0)),
            },

          new CompositeNormalizer(new Vector3(-1, -.5f, 0), new Vector3(0, 1, 0)),
          new GestureEvent(delegate (GestureMetaData data) { SetText("J", data); }));


        gestureMap["Letter-S"] = new Gesture(new List<Check> {
            new ArcCheck(new Vector3(.5f, .5f, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(-.5f,.5f,0), -90, new Vector3(0,.5f,0)),

            new ArcCheck(new Vector3(0, 0, 0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(.5f,-.5f,0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(0,-1,0), 90, new Vector3(0,-.5f,0)),
            },

          new CompositeNormalizer(new Vector3(-.5f, -1.0f, 0), new Vector3(.5f, 1.0f, 0)),
          new GestureEvent(delegate (GestureMetaData data) { SetText("S", data); }));


        gestureMap["Letter-P"] = new Gesture(new List<Check> { 
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
          new GestureEvent(delegate (GestureMetaData data) { SetText("P", data); }));



        gestureMap["Letter-Z"] = new Gesture(new List<Check> {
            new LineCheck(new Vector3(-1, 1, 0), new Vector3(1, 1, 0)),
            new LineCheck(new Vector3(1, 1, 0), new Vector3(-1, -1, 0)),
            new LineCheck(new Vector3(-1, -1, 0), new Vector3(1, -1, 0)) },

           new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
           new GestureEvent(delegate (GestureMetaData data) { SetText("Z", data); }));


        gestureMap["Triangle"] = new Gesture(new List<Check> {
            new LineCheck(new Vector3(-1, -1, 0), new Vector3(0, 1.0f, 0)),
            new LineCheck(new Vector3(0, 1.0f, 0), new Vector3(1, -1, 0)),
            new LineCheck(new Vector3(1, -1, 0),new Vector3(-1, -1, 0))},

            new CompositeNormalizer(),
            new GestureEvent(delegate (GestureMetaData data) { SetText("Triangle", data); }));


        //I am aware that any ellipse will pass this gesture. However, in testing, I found that a 1:1 aspect-ratio circle was quite difficult to make consistently
        gestureMap["Circle"] = new Gesture(new List<Check> {
            new ArcCheck(new Vector3(0, 1, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(1, 0, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(0, -1, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(-1, 0, 0), 90, new Vector3(0,0,0)),
            },

           new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
           new GestureEvent(delegate (GestureMetaData data) { SetText("Circle", data); }));




        gestureMap["Question"] = new Gesture(new List<Check> {
            new ArcCheck(new Vector3(-.5f, .5f, 0), 90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1.0f, 0), 90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(.5f, .5f, 0), 90, new Vector3(0,.5f,0)),

            new LineCheck(new Vector3(0, 0, 0),new Vector3(0, -1, 0))},

          new CompositeNormalizer(new Vector3(-.5f, -1, 0), new Vector3(.5f, 1, 0)),
          new GestureEvent(DisplayHints));


    }

    void Start () {

        if (pathRenderer == null)
        {
            pathRenderer = gameObject.AddComponent<DefaultGestureLineRenderer>();
        }

        viewNormalizer = new ViewNormalizer(Camera.main.transform); 
        dataQueue = new GTransformBuffer(512);

        GenerateGestures();
        gestureNames = new List<string>(gestureMap.Keys);

    }


    void UpdateDebugTools()
    {
        if (gestureMap.ContainsKey(displayGestureString))
        {
            if (displayGesture != null)
                displayGesture.VisualizeGesture(false);
            displayGesture = gestureMap[displayGestureString];
            displayGesture.VisualizeGesture(displayGestureVisible);
        }
    }

    void Update () {

        controller.UpdateController();

        bool gestureActive = controller.GestureActive();
        bool gestureStarted = gestureActive && !gestureActivePreviousFlag;
        bool gestureEnded = !gestureActive && gestureActivePreviousFlag;

        if (gestureActive)
        {
            PopulateQueue();
        }

        if (gestureEnded)
        {
            List<GTransform> transforms = new List<GTransform>(dataQueue);
            transforms = viewNormalizer.Normalize(transforms);
            metaData = GestureMetaData.GetGestureMetaData(transforms);

            if (renderDebugPath && displayGesture != null)
            {
                RenderList(displayGesture.normalizer.Normalize(transforms));
            }

            CheckGestures(transforms);

        } else if (gestureStarted){
            dataQueue.Clear();
            pathRenderer.SetPositions(dataQueue);
        }

        gestureActivePreviousFlag = gestureActive;


        UpdateDebugTools();
    }


    void CheckGestures(List<GTransform> transforms)
    {
        foreach (string name in gestureMap.Keys)
        {
            Gesture g = gestureMap[name];
            if (g.isEnabled && g.GestureCompleted(transforms))
            {
                g.FireEvent(metaData);
                OnGestureCompletedCallback(name);
                break;
            }
        }
    }


    void RenderList(List<GTransform> transforms)
    {
        GTransformBuffer buff = new GTransformBuffer(dataQueue.Size());
        foreach(GTransform g in transforms)
        {
            buff.Enqueue(g);
        }
        pathRenderer.SetPositions(buff);
        
    }


    void PopulateQueue(){
        GTransform nextDataPoint = controller.QueryGTransform();
        dataQueue.Enqueue(nextDataPoint);
        pathRenderer.SetPositions(dataQueue);
    }


    void AddGesture(string name, Gesture g){
        gestureMap.Add(name, g);
    }


    void SetTrackGesture(string name, bool enabled){
        if (gestureMap.ContainsKey(name)){
            gestureMap[name].isEnabled = enabled;
        }
    }


    void SetTrackGesture(List<string> names, bool enabled){
        foreach (string s in names) {
            if (gestureMap.ContainsKey(name)){
                gestureMap[name].isEnabled = enabled;
            }
        }
    }

    void OnGestureCompletedCallback(string gestureName) {
        gestureObservedCallback.Invoke(gestureName);
    }

    void AddGestureCompleteCallback(UnityAction<string> eve){
        gestureObservedCallback.AddListener(eve);
    }

    void RemoveGestureCompleteCallback(UnityAction<string> eve){
        gestureObservedCallback.RemoveListener(eve);
    }

    void RemoveAllGestureCompleteCallbacks(){
        gestureObservedCallback.RemoveAllListeners();
    }


    void SetTrackAllGestures(bool enabled){
        foreach (Gesture g in gestureMap.Values){
            g.isEnabled = enabled;
        }
    }


    private void DisplayHints(GestureMetaData data)
    {
        string newText = "Available Gestures: ";
        List<string> keys = new List<string>(gestureMap.Keys);
        for(int i = 0; i < keys.Count; i++){
            if(i%5 == 0){
                newText += "\n";
            }
            newText += keys[i] + ", ";
        }
        text.SetText(newText);
    }


    private void SetText(string name, GestureMetaData data)
    {
        string newText = "Gesture Name: " + name + "\n" +
                         "Scale: " + data.range.ToString("G4") + "\n" +
                         "Position: " + data.gestureCenter.ToString("G4");
        text.SetText(newText);
    }

}
