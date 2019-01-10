using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gesture {
    List<Check> stages;
	public Event result;
    public bool isEnabled { get; set; }
    public Normalizer normalizer;
    public UnityEvent completeEvent;
    private int currentStage;

    public Gesture(Normalizer n)
    {
        normalizer = n;
        stages = new List<Check>();
        isEnabled = true;
        currentStage = 0;
    }

    public Gesture()
    {
        stages = new List<Check>();
        isEnabled = true;
        currentStage = 0;
    }

    public Gesture(List<Check> st)
    {
        completeEvent = new UnityEvent();
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
        /*List<GTransform> normalizedTransforms = normalizer.Normalize(transforms[0]);

        foreach (GTransform g in normalizedTransforms)
        {
            bool anyCheckPasses = false;

            foreach (Check c in checks)
            {
                if (c.CheckPasses(g) == GStatus.PASS)
                {
                    anyCheckPasses = true;
                }
            }

            if (!anyCheckPasses)
                return false;

        }
        completeEvent.Invoke();*/
        return true;
    }

    public void CheckStages(GTransform gTransform)
    {
        //Debug.Log(currentStage);
        if (!isEnabled)
        {
            return;
        }
        GTransform normData = normalizer.Normalize(gTransform);

        if (currentStage >= stages.Count)
        {
            Debug.Log("Gesture complete");
            CompleteGesture();
            currentStage = 0;
            //isEnabled = false;
        }
        else
        {
            switch (stages[currentStage].CheckPoint(normData))
            {
                case GStatus.PASS:
                    Debug.Log(normData.position);
                    currentStage++;
                    break;
                case GStatus.HALT:
                    Debug.Log(normData.position);
                    currentStage = 0;
                    break;
            }
        }
    }


    public void AddStage(Check stage)
    {
        stages.Add(stage);
    }





    void CompleteGesture()
    {
        completeEvent.Invoke();
    }

    public void AddEvent(UnityEvent eventAction)
    {
        completeEvent = eventAction;
    }

    void ClearEvents()
    {
        completeEvent.RemoveAllListeners();
    }



}
