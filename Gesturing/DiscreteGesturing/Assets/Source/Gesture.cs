using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gestures
{
    public class Gesture
    {
        List<Check> checks;
        List<Check> sequentialChecks;

        public bool isEnabled { get; set; }
        public Normalizer normalizer;
        public UnityEvent<GestureMetaData> completeEvent;
        private float gestureCompleteConfidence = 1.0f;
        public static GameObject gestureVisualContainer;

        public Gesture(List<Check> st, Normalizer norm, UnityEvent<GestureMetaData> eve)
        {
            normalizer = norm;
            completeEvent = eve;
            checks = st;
            sequentialChecks = new List<Check>();

            isEnabled = true;
        }

        public Gesture(List<Check> stages, List<Check> sequential, Normalizer norm, UnityEvent<GestureMetaData> eve) : this(stages, norm, eve)
        {
            sequentialChecks = sequential;
        }


        void AddStage(Check newCheck)
        {
            checks.Add(newCheck);
        }

        public void VisualizeGesture(bool active)
        {
            foreach (Check c in checks)
            {
                c.VisualizeCheck(active);
            }

            foreach (Check c in sequentialChecks)
            {
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

            foreach (GTransform g in normalizedTransforms)
            {
                if (sequentialCheckPtr < sequentialChecks.Count)
                {
                    Check currentSequentialCheck = sequentialChecks[sequentialCheckPtr];
                    if (currentSequentialCheck.CheckPasses(g) == GStatus.PASS)
                    {
                        sequentialCheckPtr++;

                    }
                }

                for (int i = 0; i < checks.Count; i++)
                {
                    Check c = checks[i];
                    if (c.CheckPasses(g) == GStatus.PASS)
                    {
                        checksPassed++;
                        checkHitList[i] = true;
                        break;
                    }
                }

            }

            if (sequentialCheckPtr == sequentialChecks.Count &&
                checkHitList.TrueForAll(b => b) &&
                ((float)checksPassed / transformCount) >= gestureCompleteConfidence)
            {
                return true;
            }

            return false;
        }


        public void FireEvent(GestureMetaData metaData)
        {
            completeEvent.Invoke(metaData);
        }


        void AddEvent(UnityAction<GestureMetaData> eventAction)
        {
            completeEvent.AddListener(eventAction);
        }

        void ClearEvents()
        {
            completeEvent.RemoveAllListeners();
        }



    }
}
