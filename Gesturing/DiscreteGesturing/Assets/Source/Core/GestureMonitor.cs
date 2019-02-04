using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gestures
{
    public class GestureMonitor : MonoBehaviour
    {
        public LineRenderer pathRenderer;
        public IController controller;

        private Normalizer viewNormalizer;
        private Dictionary<string, Gesture> gestureMap = new Dictionary<string, Gesture>();
        public List<string> gestureNames;
        private GTransformBuffer dataQueue;
        public int maxBufferSize = 512;
        private bool gestureActivePreviousFlag = false;

        private Gesture displayGesture = null;
        public string displayGestureString = "";
        public bool displayGestureVisible = true;
        public bool renderDebugPath = true;

        private UnityEvent<GestureMetaData> gestureObservedCallback;
        private UnityEvent<GestureMetaData> gestureFailedCallback;
        private GestureMetaData metaData;



        void Awake()
        {
            viewNormalizer = viewNormalizer ?? new ViewNormalizer(Camera.main.transform);
            gestureObservedCallback = new GestureEvent();
            gestureFailedCallback = new GestureEvent();
            dataQueue = new GTransformBuffer(maxBufferSize);


            Gesture.gestureVisualContainer = new GameObject();
            Gesture.gestureVisualContainer.name = "Gesture Visual Container";

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

        void Update()
        {
            controller.UpdateController();

            bool gestureActive = controller.GestureActive();
            bool gestureStarted = gestureActive && !gestureActivePreviousFlag;
            bool gestureEnded = !gestureActive && gestureActivePreviousFlag;

            if (gestureActive){
                PopulateQueue();
            }

            if (gestureEnded){
                List<GTransform> transforms = new List<GTransform>(dataQueue);
                transforms = viewNormalizer.Normalize(transforms);
                metaData = GestureMetaData.GetGestureMetaData(transforms);

                if (renderDebugPath && displayGesture != null)
                {
                    RenderList(displayGesture.normalizer.Normalize(transforms));
                }

                CheckGestures(transforms);
            }

            else if (gestureStarted){
                dataQueue.Clear();
                SetLineRendererPositions(dataQueue);
            }

            gestureActivePreviousFlag = gestureActive;

            UpdateDebugTools();
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
