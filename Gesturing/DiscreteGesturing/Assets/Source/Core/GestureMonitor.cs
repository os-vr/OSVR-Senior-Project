using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gestures
{
    [ExecuteInEditMode]
    public class GestureMonitor : MonoBehaviour
    {
        public LineRenderer pathRenderer;
        public IController controller;

        private Normalizer viewNormalizer;
        private Dictionary<string, Gesture> gestureMap = new Dictionary<string, Gesture>();
        public List<string> gestureNames;
        private GTransformBuffer dataQueue;
        public int bufferSize = 512;
        private bool gestureActivePreviousFlag = false;

        private UnityEvent<GestureMetaData> gestureObservedCallback;
        private UnityEvent<GestureMetaData> gestureFailedCallback;
        private GestureMetaData metaData;

        //public List<LineCheck> list = new List<LineCheck>();

        
        void OnEnable() {
            AddGesture("sq", new SquareGesture());


            AddGesture("Vertical",
          new Gesture(new List<Check> {
               new LineCheck(new Vector3(0, 1, 0), new Vector3(0, -1, 0)) },
              new LineNormalizer(),
              new GestureEvent()));


            AddGesture("Horizontal",
               new Gesture(new List<Check> {
               new LineCheck(new Vector3(-1, 0, 0), new Vector3(1, 0, 0)) },
                   new LineNormalizer(),
                   new GestureEvent()));


            AddGesture("Square", new SquareGesture());


            AddGesture("Letter-J", new Gesture(new List<Check> {
            new LineCheck(new Vector3(0, 1, 0), new Vector3(0, 0, 0)),
            new ArcCheck(new Vector3(0, 0, 0), 90, new Vector3(-.5f,0,0)),
            new ArcCheck(new Vector3(-.5f, -.5f, 0), 90, new Vector3(-.5f,0,0)),
            },

              new CompositeNormalizer(new Vector3(-1, -.5f, 0), new Vector3(0, 1, 0)),
              new GestureEvent()));


            AddGesture("2", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(.5f, .5f, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1, 0), -90, new Vector3(0,.5f,0)),
            new LineCheck(new Vector3(.5f, .5f, 0), new Vector3(-.5f, -1, 0)),
            new LineCheck(new Vector3(-.5f, -1, 0), new Vector3(.5f, -1, 0)),
            },

              new CompositeNormalizer(new Vector3(-.5f, -1, 0), new Vector3(.5f, 1, 0)),
              new GestureEvent()));


            AddGesture("3", new Gesture(new List<Check> {
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


            AddGesture("4", new Gesture(new List<Check> {
            new LineCheck(new Vector3(0, -1, 0), new Vector3(0, 1, 0)),
            new LineCheck(new Vector3(0, 1, 0), new Vector3(-.5f, 0, 0)),
            new LineCheck(new Vector3(-.5f, 0, 0), new Vector3(.5f, 0, 0)),
            },

            new CompositeNormalizer(new Vector3(-.5f, -1, 0), new Vector3(.5f, 1, 0)),
            new GestureEvent()));


            AddGesture("Letter-S", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(.5f, .5f, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(-.5f,.5f,0), -90, new Vector3(0,.5f,0)),

            new ArcCheck(new Vector3(0, 0, 0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(.5f,-.5f,0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(0,-1,0), 90, new Vector3(0,-.5f,0)),
            },

              new CompositeNormalizer(new Vector3(-.5f, -1.0f, 0), new Vector3(.5f, 1.0f, 0)),
              new GestureEvent()));


            AddGesture("Letter-P", new Gesture(new List<Check> {
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



            AddGesture("Letter-Z", new Gesture(new List<Check> {
            new LineCheck(new Vector3(-1, 1, 0), new Vector3(1, 1, 0)),
            new LineCheck(new Vector3(1, 1, 0), new Vector3(-1, -1, 0)),
            new LineCheck(new Vector3(-1, -1, 0), new Vector3(1, -1, 0)) },

               new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
               new GestureEvent()));


            AddGesture("Triangle", new Gesture(new List<Check> {
            new LineCheck(new Vector3(-1, -1, 0), new Vector3(0, 1.0f, 0)),
            new LineCheck(new Vector3(0, 1.0f, 0), new Vector3(1, -1, 0)),
            new LineCheck(new Vector3(1, -1, 0),new Vector3(-1, -1, 0))},

                new CompositeNormalizer(),
                new GestureEvent()));


            AddGesture("Circle", new Gesture(new List<Check> {
            new ArcCheck(new Vector3(0, 1, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(1, 0, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(0, -1, 0), 90, new Vector3(0,0,0)),
            new ArcCheck(new Vector3(-1, 0, 0), 90, new Vector3(0,0,0)),
            },

               new CompositeNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
               new GestureEvent()));

        }


        void Awake()
        {
            viewNormalizer = viewNormalizer ?? new ViewNormalizer(Camera.main.transform);
            gestureObservedCallback = new GestureEvent();
            gestureFailedCallback = new GestureEvent();
            dataQueue = new GTransformBuffer(bufferSize);
        }

        void Update()
        {
            bool gestureActive = controller.GestureActive();
            bool gestureStarted = gestureActive && !gestureActivePreviousFlag;
            bool gestureEnded = !gestureActive && gestureActivePreviousFlag;

            if (gestureActive){
                PopulateQueue();
            }

            if (gestureEnded){
                List<GTransform> transforms = new List<GTransform>(dataQueue);
                metaData = GestureMetaData.GetGestureMetaData(transforms);
                transforms = viewNormalizer.Normalize(transforms);


                CheckGestures(transforms);
            }

            else if (gestureStarted){
                dataQueue.Clear();
                SetLineRendererPositions(dataQueue);
            }

            gestureActivePreviousFlag = gestureActive;

        }


        void CheckGestures(List<GTransform> transforms)
        {
            bool gestureCompleted = false;
            foreach (string name in gestureMap.Keys)
            {
                Gesture g = gestureMap[name];
                if (g.isEnabled && g.GestureCompleted(transforms))
                {
                    metaData.gestureName = name;
                    metaData.precision = g.gestureCompletionPrecision;
                    g.FireEvent(metaData);
                    OnGestureCompletedCallback(metaData);
                    gestureCompleted = true;
                    break;
                }
            }
            if (!gestureCompleted) {
                OnGestureFailedCallback(metaData);
            }
        }


        void RenderList(List<GTransform> transforms)
        {
            GTransformBuffer buff = new GTransformBuffer(dataQueue.Size());
            foreach (GTransform g in transforms)
            {
                buff.Enqueue(g);
            }
            SetLineRendererPositions(buff);

        }


        void PopulateQueue()
        {
            GTransform nextDataPoint = controller.QueryGTransform();
            dataQueue.Enqueue(nextDataPoint);
            SetLineRendererPositions(dataQueue);
        }


        void SetLineRendererPositions(GTransformBuffer queue) {
            if (pathRenderer != null) {
                Vector3[] arr = queue.ToArray();
                pathRenderer.positionCount = arr.Length;
                pathRenderer.SetPositions(arr);
            }
        }


        public Dictionary<string, Gesture> GetGestureMap() {
            return gestureMap;
        }


        public void AddGesture(string name, Gesture g){
            gestureMap.Add(name, g);
            gestureNames = new List<string>(gestureMap.Keys);
        }


        public void SetTrackGesture(string gname, bool enabled)
        {
            Gesture g;
            gestureMap.TryGetValue(gname, out g);
            if (g != null) {
                g.isEnabled = enabled;
            }
        }


        public void SetTrackGesture(List<string> names, bool enabled){
            foreach (string s in names){
                SetTrackGesture(s, enabled);
            }
        }

        public void SetTrackAllGestures(bool enabled) {
            foreach (Gesture g in gestureMap.Values) {
                g.isEnabled = enabled;
            }
        }


        void OnGestureCompletedCallback(GestureMetaData metaData){
            gestureObservedCallback.Invoke(metaData);
        }

        public void AddGestureCompleteCallback(UnityAction<GestureMetaData> eve){
            gestureObservedCallback.AddListener(eve);
        }

        public void RemoveGestureCompleteCallback(UnityAction<GestureMetaData> eve){
            gestureObservedCallback.RemoveListener(eve);
        }

        public void RemoveAllGestureCompleteCallbacks(){
            gestureObservedCallback.RemoveAllListeners();
        }




        void OnGestureFailedCallback(GestureMetaData metaData) {
            gestureFailedCallback.Invoke(metaData);
        }

        public void AddGestureFailedCallback(UnityAction<GestureMetaData> eve) {
            gestureFailedCallback.AddListener(eve);
        }

        public void RemoveGestureFailedCallback(UnityAction<GestureMetaData> eve) {
            gestureFailedCallback.RemoveListener(eve);
        }

        public void RemoveAllGestureFailedCallbacks() {
            gestureFailedCallback.RemoveAllListeners();
        }



        public void SetMaxBufferSize(int size) {
            dataQueue.SetMaxSize(size);
        }

        public void SetBufferWrap(bool circular) {
            dataQueue.SetCircular(circular);
        }

    }
}
