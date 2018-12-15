using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gesture {
    List<Stage> stages;
	public Event result;
    public bool isEnabled { get; set; }
    public Normalizer normalizer;
    public UnityEvent completeEvent;
    private int currentStage;

    public Gesture(Normalizer n)
    {
        normalizer = n;
        stages = new List<Stage>();
        isEnabled = true;
        currentStage = 0;
    }

    public Gesture()
    {
        stages = new List<Stage>();
        isEnabled = true;
        currentStage = 0;
    }

    public Gesture(List<Stage> st)
    {
        completeEvent = new UnityEvent();
        currentStage = 0;

        stages = new List<Stage>();
        isEnabled = true;
        foreach(Stage s in st)
        {
            AddStage(s);
        }
        
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
            switch (stages[currentStage].Passes(normData))
            {
                case GSuccess.PASS:
                    Debug.Log(normData.position);
                    currentStage++;
                    break;
                case GSuccess.HALT:
                    Debug.Log(normData.position);
                    currentStage = 0;
                    break;
            }
        }
    }


    public void AddStage(Stage stage)
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
