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

    public Gesture(List<Stage> st)
    {
        completeEvent = new UnityEvent();

        stages = new List<Stage>();
        isEnabled = true;
        foreach(Stage s in st)
        {
            AddStage(s);
        }
        
    }

    public void CheckStages(GTransform gTransform)
    {
        stages[0].Passes(gTransform);
    }


    void AddStage(Stage stage)
    {
        stages.Add(stage);
    }





    void CompleteGesture()
    {
        completeEvent.Invoke();
    }

    void AddEvent(UnityAction eventAction)
    {
        completeEvent.AddListener(eventAction);
    }

    void ClearEvents()
    {
        completeEvent.RemoveAllListeners();
    }



}
