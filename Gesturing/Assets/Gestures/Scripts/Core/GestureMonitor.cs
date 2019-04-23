using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gestures {
    /// <summary>
    /// Class responsible for tracking gestures. See <see cref="TrackerSetup"/> for example of how to set up instance.
    /// </summary>
    public class GestureMonitor : MonoBehaviour
    {
        /// <summary>
        /// A LineRenderer used to visualize the gestures during and after gesture creation.
        /// </summary>
        public LineRenderer lineRenderer;
        /// <summary>
        /// The IController that generates the GTransform objects for gesture recognition.
        /// </summary>
        public IController controller;

        /// <summary>
        /// The default buffer size for the GTransformBuffer class.
        /// </summary>
        public int bufferSize = 512;

        private Normalizer viewNormalizer; // default normalizer for gesture creation=
        private Dictionary<string, Gesture> gestureMap = new Dictionary<string, Gesture>();
        private GTransformBuffer dataQueue;

        private UnityEvent<GestureMetaData> gestureObservedCallback;
        private UnityEvent<GestureMetaData> gestureFailedCallback;
        private UnityEvent gestureStartCallback;
        private GestureMetaData metaData;

        private bool gestureActivePreviousFlag = false;



        public void Awake() {
            gestureObservedCallback = new GestureEvent();
            gestureFailedCallback = new GestureEvent();
            gestureStartCallback = new UnityEvent();
            viewNormalizer = new ViewNormalizer(Camera.main.transform);
            dataQueue = new GTransformBuffer(bufferSize);
        }


        public void Update()
        {
            if(controller == null) {
                return;
            }

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
                OnGestureStartCallback();
            }

            gestureActivePreviousFlag = gestureActive;

        }


        private void CheckGestures(List<GTransform> transforms)
        {
            bool gestureCompleted = false;
            foreach (string name in gestureMap.Keys)
            {
                Gesture g = gestureMap[name];
                if (g.isEnabled && g.GestureCompleted(transforms))
                {
                    metaData.name = name;
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

        /// <summary>
        /// Debug method to set the linerenderer positions
        /// </summary>
        /// <param name="transforms"></param>
        private void RenderList(List<GTransform> transforms)
        {
            GTransformBuffer buff = new GTransformBuffer(dataQueue.Size());
            foreach (GTransform g in transforms)
            {
                buff.Enqueue(g);
            }
            SetLineRendererPositions(buff);
        }


        private void PopulateQueue()
        {
            GTransform nextDataPoint = controller.QueryGTransform();
            dataQueue.Enqueue(nextDataPoint);
            SetLineRendererPositions(dataQueue);
        }


        private void SetLineRendererPositions(GTransformBuffer queue) {
            if (lineRenderer != null) {
                Vector3[] arr = queue.ToArray();
                lineRenderer.positionCount = arr.Length;
                lineRenderer.SetPositions(arr);
            }
        }

        /// <summary>
        /// Get the Dictionary storing Gestures and names.
        /// </summary>
        /// <returns>Dictionary mapping Gesture names to gestures</returns>
        public Dictionary<string, Gesture> GetGestureMap() {
            return gestureMap;
        }

        /// <summary>
        /// Get the data normalizer used by this monitor
        /// </summary>
        /// <returns>Return the class's ViewNormalizer</returns>
        public Normalizer GetViewNormalizer() {
            return viewNormalizer;
        }


        /// <summary>
        /// Sets the normalizer used for pre-processing the datapoints before the individual gestures' normaization takes place.
        /// </summary>
        /// <param name="norm"></param>
        /// <returns></returns>
        public void SetViewNormalizer(Normalizer norm)
        {
            this.viewNormalizer = norm;
        }

        /// <summary>
        /// Get the data buffer used by this monitor
        /// </summary>
        /// <returns>Return the class's GTransformBuffer</returns>
        public GTransformBuffer GetDataBuffer() {
            return dataQueue;
        }

        /// <summary>
        /// Add a new Gesture to be tracked
        /// </summary>
        /// <param name="name">The name to reference the Gesture by</param>
        /// <param name="g">The instance of a Gesture to track</param>
        public void AddGesture(string name, Gesture g){
            gestureMap.Add(name, g);
        }


        /// <summary>
        /// Set the tracking state for a single Gesture
        /// </summary>
        /// <param name="gname">The name of the Gesture to set the tracking state of</param>
        /// <param name="enabled">`True` if the Gesture should be enabled, `False` otherwise</param>
        public void SetTrackGesture(string gname, bool enabled)
        {
            Gesture g;
            gestureMap.TryGetValue(gname, out g);
            if (g != null) {
                g.isEnabled = enabled;
            }
        }

        /// <summary>
        /// Set the tracking state for a list of Gestures
        /// </summary>
        /// <param name="names">The names of all Gestures to set the tracking state of</param>
        /// <param name="enabled">`True` if the Gestures should be enabled, `False` otherwise</param>
        public void SetTrackGesture(List<string> names, bool enabled){
            foreach (string s in names){
                SetTrackGesture(s, enabled);
            }
        }

        /// <summary>
        /// Set the tracking state for all Gestures
        /// </summary>
        /// <param name="enabled">`True` if all Gestures should be enabled, `False` otherwise</param>
        public void SetTrackAllGestures(bool enabled) {
            foreach (Gesture g in gestureMap.Values) {
                g.isEnabled = enabled;
            }
        }


        void OnGestureCompletedCallback(GestureMetaData metaData){
            gestureObservedCallback.Invoke(metaData);
        }

        /// <summary>
        /// Add an event listener for when a user completes a valid Gesture
        /// </summary>
        /// <param name="eve"></param>
        public void AddGestureCompleteCallback(UnityAction<GestureMetaData> eve){
            gestureObservedCallback.AddListener(eve);
        }

        /// <summary>
        /// Remove an event listener for when a user completes a valid Gesture
        /// </summary>
        /// <param name="eve"></param>
        public void RemoveGestureCompleteCallback(UnityAction<GestureMetaData> eve){
            gestureObservedCallback.RemoveListener(eve);
        }

        /// <summary>
        /// Remove all event listeners for when a user completes a valid Gesture
        /// </summary>
        public void RemoveAllGestureCompleteCallbacks(){
            gestureObservedCallback.RemoveAllListeners();
        }



        void OnGestureFailedCallback(GestureMetaData metaData) {
            gestureFailedCallback.Invoke(metaData);
        }

        /// <summary>
        /// Add an event listener for when a user fails to complete a valid Gesture
        /// </summary>
        /// <param name="eve"></param>
        public void AddGestureFailedCallback(UnityAction<GestureMetaData> eve) {
            gestureFailedCallback.AddListener(eve);
        }

        /// <summary>
        /// Remove an event listener for when a user fails to complete a valid Gesture
        /// </summary>
        /// <param name="eve"></param>
        public void RemoveGestureFailedCallback(UnityAction<GestureMetaData> eve) {
            gestureFailedCallback.RemoveListener(eve);
        }

        /// <summary>
        /// Remove all event listeners for when a user fails to complete a valid Gesture
        /// </summary>
        public void RemoveAllGestureFailedCallbacks() {
            gestureFailedCallback.RemoveAllListeners();
        }


        void OnGestureStartCallback() {
            gestureStartCallback.Invoke();
        }

        /// <summary>
        /// Add an event listener for when a user starts drawing a Gesture
        /// </summary>
        public void AddGestureStartCallback(UnityAction eve) {
            gestureStartCallback.AddListener(eve);
        }

        /// <summary>
        /// Remove an event listeners for when a user starts drawing a Gesture
        /// </summary>
        public void RemoveGestureStartCallback(UnityAction eve) {
            gestureStartCallback.RemoveListener(eve);
        }

        /// <summary>
        /// Remove all event listeners for when a user starts drawing a Gesture
        /// </summary>
        public void RemoveAllGestureStartCallbacks() {
            gestureStartCallback.RemoveAllListeners();
        }


        /// <summary>
        /// Set the max capacity of the transform buffer
        /// </summary>
        /// <param name="size">Set the number of GTransforms that the buffer can hold. Default is 512</param>
        public void SetMaxBufferSize(int size) {
            dataQueue.SetMaxSize(size);
        }

        /// <summary>
        /// Set whether the buffer should act as a standard array or a circular array
        /// </summary>
        /// <param name="circular">`True` if the buffer should wrap, `False` otherwise</param>
        public void SetBufferWrap(bool circular) {
            dataQueue.SetCircular(circular);
        }

        /// <summary>
        /// Clear the buffer of all Transforms and erase all positions from the attached LineRenderer
        /// </summary>
        public void ClearBuffer() {
            dataQueue.Clear();
            SetLineRendererPositions(dataQueue);
        }

    }
}
