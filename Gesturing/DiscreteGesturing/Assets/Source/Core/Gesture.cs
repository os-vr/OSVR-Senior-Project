using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gestures
{

    public class Gesture
    {
        public List<Check> checks;
        public List<Check> sequentialChecks;

        public bool isEnabled;
        public Normalizer normalizer;
        public UnityEvent<GestureMetaData> completeEvent;
        public float gestureCompleteConfidence = 1.0f;
        public float gestureCompletionPrecision = -1.0f;

        public static GameObject gestureVisualContainer;

        public Gesture(List<Check> st, Normalizer norm, UnityEvent<GestureMetaData> eve)
        {
            normalizer = norm;
            completeEvent = eve;
            checks = st;
            sequentialChecks = new List<Check>();

            isEnabled = true;
        }

        public Gesture(List<Check> stages, List<Check> sequential, Normalizer norm, UnityEvent<GestureMetaData> eve) : this(stages, norm, eve) {
            sequentialChecks = sequential;
        }


        void AddStage(Check newCheck) {
            checks.Add(newCheck);
        }

        public void VisualizeGesture(bool active)
        {
            foreach (Check c in checks) {
                c.VisualizeCheck(active);
            }

            foreach (Check c in sequentialChecks) {
                c.VisualizeCheck(active);
            }

        }


        public bool GestureCompleted(List<GTransform> transforms)
        {
            List<GTransform> normalizedTransforms = normalizer.Normalize(transforms);
            int sequentialCheckPtr = 0;
            List<bool> checkHitList = new List<bool>(new bool[checks.Count]);

            int transformCount = normalizedTransforms.Count;
            int checksPassed = 0;
            gestureCompletionPrecision = 0.0f;


            foreach (GTransform g in normalizedTransforms)
            {
                if (sequentialCheckPtr < sequentialChecks.Count)
                {
                    Check currentSequentialCheck = sequentialChecks[sequentialCheckPtr];
                    if (currentSequentialCheck.CheckPasses(g) != -1)
                    {
                        sequentialCheckPtr++;

                    }
                }

                bool anyCheckPassed = false;
                float min = 1.0f;
                for (int i = 0; i < checks.Count; i++)
                {
                    Check c = checks[i];
                    float f = c.CheckPasses(g);
                    if (f != -1)
                    {
                        min = Mathf.Min(f, min);
                        checkHitList[i] = true;
                        anyCheckPassed = true;
                    }
                }
                if (anyCheckPassed) {
                    checksPassed++;
                }
                gestureCompletionPrecision += min;

            }

            gestureCompletionPrecision /= transformCount;

            if (sequentialCheckPtr == sequentialChecks.Count &&
                checkHitList.TrueForAll(b => b) &&
                ((float)checksPassed / transformCount) >= gestureCompleteConfidence)
            {
                return true;
            }
            return false;
        }


        public void FireEvent(GestureMetaData metaData) {
            completeEvent.Invoke(metaData);
        }

        void AddEvent(UnityAction<GestureMetaData> eventAction) {
            completeEvent.AddListener(eventAction);
        }

        void ClearEvents() {
            completeEvent.RemoveAllListeners();
        }

        public static Transform GetVisualContainerTransform() {
            if(gestureVisualContainer == null) {
                gestureVisualContainer = new GameObject("Gesture Visualization OBJ");
            }
            return gestureVisualContainer.transform;
        }



    }
}
