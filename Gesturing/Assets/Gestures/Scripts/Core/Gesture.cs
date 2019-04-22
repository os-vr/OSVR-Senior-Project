﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Gestures {
    /// <summary>
    /// A  class that is composed of Checks to represent a gesture.
    /// </summary>
    public class Gesture {

        private List<Check> checks;
        private List<Check> sequentialChecks;
        private List<Check> alwaysChecks;

        /// <summary>
        /// Whether the gesture is actively being detected or not.
        /// </summary>
        public bool isEnabled { get; set; }

        /// <summary>
        /// The ratio of points that needs to satisfy gesture checks for gesture completion.
        /// </summary>
        /// <remarks>
        /// Highly advised to leave at 1, or remain above 0.9. Low values will cause unwanted side effects like gesture overlap, or acceptance of random movements as gestures.
        /// </remarks>
        public float gestureCompleteConfidence { get; set; }


        private Normalizer normalizer;
        private UnityEvent<GestureMetaData> completeEvent;

        /// <summary>
        /// A measure of how close a set of GTransforms is to the gesture definition.
        /// </summary>
        /// <remarks>
        /// 0.0 is a perfect gesture. Humanly impossible. 1.0 is as imperfect as possible while still passing the gesture at full confidence.
        /// When gesture confidence is not 1, the value is `actual gesture completion precision minus the proportion of points satisfing gesture`.
        /// 
        /// </remarks>
        public float gestureCompletionPrecision {get; set;}

        /// <summary>
        /// Create a Gesture object. Use additional methods to specify gesture composition for events, checks, and normalizers.
        /// </summary>
        public Gesture() {
            normalizer = null;
            completeEvent = new GestureEvent();
            checks = new List<Check>();
            sequentialChecks = new List<Check>();
            alwaysChecks = new List<Check>();

            gestureCompleteConfidence = 1;
            isEnabled = true;
        }

        /// <summary>
        /// Determines if the specified GTransforms generated satisfy the requirements of this gesture.
        /// </summary>
        /// <param name="transforms">The List of Transforms to be checked</param>
        /// <returns></returns>
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

        /// <summary>
        /// Fires the specifie GestureEvent associated with this gesture.
        /// </summary>
        /// <param name="metaData">The GestureMetaData created from the gesture data.</param>
        public void FireEvent(GestureMetaData metaData) {
            completeEvent.Invoke(metaData);
        }

        /// <summary>
        /// Adds a GestureEvent to be called at gesture completion.
        /// </summary>
        /// <param name="eventAction"></param>
        /// <returns></returns>
        public Gesture AddEvent(UnityAction<GestureMetaData> eventAction) {
            completeEvent.AddListener(eventAction);
            return this;
        }

        /// <summary>
        /// Removes all events associated with this gesture.
        /// </summary>
        public void ClearEvents() {
            completeEvent.RemoveAllListeners();
        }
        /// <summary>
        /// Sets the Normalizer to be applied to every incoming GTransform data point.
        /// </summary>
        /// <param name="normalizer">The Normalizer to be set.</param>
        public Gesture SetNormalizer(Normalizer normalizer) {
            normalizer = nm;
            return this;
        }

        /// <summary>
        /// Adds a single check to the list of checks that only need to be satisfied once for gesture completion.
        /// </summary>
        /// <param name="newCheck"></param>
        /// <returns></returns>
        public Gesture AddOnceCheck(Check newCheck) {
            checks.Add(newCheck);
            return this;
        }

        /// <summary>
        /// Adds a range of checks to the list of checks that only need to be satisfied once for gesture completion.
        /// </summary>
        /// <param name="newChecks">IEnumerator of Checks</param>
        /// <returns></returns>
        public Gesture AddOnceChecks(IEnumerable<Check> newChecks) {
            checks.AddRange(newChecks);
            return this;
        }

        /// <summary>
        /// Adds a single check to the list of checks that need to be satisfied by every data point for gesture completion.
        /// </summary>
        /// <param name="newCheck">The check to be added to the always-check list.</param>
        /// <returns></returns>
        public Gesture AddAlwaysCheck(Check newCheck) {
            alwaysChecks.Add(newCheck);
            return this;
        }

        /// <summary>
        /// Adds a range of check to the list of checks that need to be satisfied by every data point for gesture completion.
        /// </summary>
        /// <param name="newChecks">IEnumerable of Checks to be passed in.</param>
        /// <returns></returns>
        public Gesture AddAlwaysChecks(IEnumerable<Check> newChecks) {
            alwaysChecks.AddRange(newChecks);
            return this;
        }

        /// <summary>
        /// Adds a single check to the list of checks that must be satisfied in sequential order for gesture completion.
        /// </summary>
        /// <param name="newCheck">The Check to be added.</param>
        /// <returns></returns>
        public Gesture AddSequentialCheck(Check newCheck) {
            sequentialChecks.Add(newCheck);
            return this;
        }

        /// <summary>
        /// Adds a range of checks to the list of chekcs that must be satisfied in sequential order for gesture completion.
        /// </summary>
        /// <param name="newChecks">IEnumerable of Checks to be passed in.</param>
        /// <returns></returns>
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
