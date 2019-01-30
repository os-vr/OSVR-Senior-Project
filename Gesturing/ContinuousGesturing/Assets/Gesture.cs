using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gesture {
    protected List<Check> stages;
    public bool isEnabled { get; set; }
    public Normalizer normalizer;
    public GestureEvent completeEvent;
    protected int currentStage;
    protected List<GTransform> usedPoints;

    public Gesture(Normalizer n)
    {
        usedPoints = new List<GTransform>();
        normalizer = n;
        stages = new List<Check>();
        isEnabled = true;
        currentStage = 0;
    }

    public Gesture()
    {
        usedPoints = new List<GTransform>();
        stages = new List<Check>();
        isEnabled = true;
        currentStage = 0;
    }

    public Gesture(List<Check> st)
    {
        usedPoints = new List<GTransform>();
        currentStage = 0;

        stages = new List<Check>();
        isEnabled = true;
        foreach(Check s in st)
        {
            AddStage(s);
        }
        
    }

    public bool GestureCompleted(List<GTransform> transforms)
    {
        return true;
    }

    public virtual bool CheckStages(GTransform gTransform)
    {
        GTransform normData = normalizer.Normalize(gTransform);
        if (currentStage >= stages.Count)
        {
            Debug.Log("Gesture complete");
            GestureMetaData gmd = GestureMetaData.GetGestureMetaData(usedPoints);
            CompleteGesture(gmd);
            

            currentStage = 0;
            return true;
        }
        else
        {
            //Debug.Log(normData.position);
            usedPoints.Add(gTransform);
            switch (stages[currentStage].CheckPoint(normData))
            {
                case GStatus.PASS:
                    Debug.Log(normData.position);
                    currentStage++;
                    break;
                case GStatus.HALT:
                    Debug.Log(normData.position);
                    Debug.Log("resetting");
                    currentStage = 0;
                    usedPoints.Clear();
                    break;
            }
        }
        return false;
    }


    public void AddStage(Check stage)
    {
        stages.Add(stage);
    }





    protected void CompleteGesture(GestureMetaData data)
    {
        completeEvent.Invoke(data);
    }

    public void AddEvent(GestureEvent eventAction)
    {
        completeEvent = eventAction;
    }

    protected void ClearEvents()
    {
        completeEvent.RemoveAllListeners();
    }



}
