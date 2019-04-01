using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gestures
{
    public class Gesture {
        public List<Check> checks;
        public List<Check> sequentialChecks;
        public List<Check> alwaysChecks;

        public bool isEnabled { get; set; }
        public Normalizer normalizer;
        public UnityEvent<GestureMetaData> completeEvent;

        public float gestureCompleteConfidence { get; set; }
        public float gestureCompletionPrecision = -1.0f;

        public Gesture() {
            normalizer = null;
            completeEvent = new GestureEvent();
            checks = new List<Check>();
            sequentialChecks = new List<Check>();
            alwaysChecks = new List<Check>();

            gestureCompleteConfidence = 1;
            isEnabled = true;
        }

        // work on deprecation
        public Gesture(List<Check> st, Normalizer norm, UnityEvent<GestureMetaData> eve)
        {
            normalizer = norm;
            completeEvent = eve;
            checks = st;
            sequentialChecks = new List<Check>();
            alwaysChecks = new List<Check>();

            gestureCompleteConfidence = 1;
            isEnabled = true;
        }


        public bool GestureCompleted(List<GTransform> transforms)
        {
            List<GTransform> normalizedTransforms = new List<GTransform>(transforms);

            if (normalizer != null) {
                normalizedTransforms = normalizer.Normalize(normalizedTransforms);
            }

            int sequentialCheckPtr = 0;
            List<bool> checkHitList = new List<bool>(new bool[checks.Count]);

            int transformCount = normalizedTransforms.Count;
            int checksPassed = 0;
            int counter = 0;
            gestureCompletionPrecision = 0.0f;


            foreach (GTransform g in normalizedTransforms)
            {
                counter++;
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
                

                bool anyCheckFailed = false;
                for (int i = 0; i < alwaysChecks.Count; i++) {
                    Check c = alwaysChecks[i];
                    float f = c.CheckPasses(g);
                    if (f == -1) {
                        anyCheckFailed = true;
                        break;
                    }
                }


                if (anyCheckPassed && !anyCheckFailed) {
                    checksPassed++;
                }
                gestureCompletionPrecision += min;
                if ((checksPassed + (transformCount - counter))/(float)transformCount < gestureCompleteConfidence)
                {
                    // if expected confidence cannot be achieved, short-circuit
                    return false;
                }


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

        public Gesture AddEvent(UnityAction<GestureMetaData> eventAction) {
            completeEvent.AddListener(eventAction);
            return this;
        }

        public void ClearEvents() {
            completeEvent.RemoveAllListeners();
        }

        public Gesture SetNormalizer(Normalizer nm) {
            normalizer = nm;
            return this;
        }

        public Gesture AddOnceCheck(Check newCheck) {
            checks.Add(newCheck);
            return this;
        }

        public Gesture AddOnceChecks(IEnumerable<Check> newChecks) {
            checks.AddRange(newChecks);
            return this;
        }

        public Gesture AddAlwaysCheck(Check newCheck) {
            alwaysChecks.Add(newCheck);
            return this;
        }

        public Gesture AddAlwaysChecks(IEnumerable<Check> newChecks) {
            alwaysChecks.AddRange(newChecks);
            return this;
        }

        public Gesture AddSequentialCheck(Check newCheck) {
            sequentialChecks.Add(newCheck);
            return this;
        }

        public Gesture AddSequentialChecks(IEnumerable<Check> newChecks) {
            sequentialChecks.AddRange(newChecks);
            return this;
        }
        
        public void VisualizeGesture(Rect grid) {
            foreach (Check c in checks) {
                c.VisualizeCheck(grid);
            }

            foreach (Check c in sequentialChecks) {
                c.VisualizeCheck(grid);
            }
        }

    }
}
