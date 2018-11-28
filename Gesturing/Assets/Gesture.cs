using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gesture {
    List<Stage> stages;
	public Event result;
    bool isEnabled { get; set; }
    public Normalizer normalizer;

    Gesture(List<Stage> stages)
    {
        stages = new List<Stage>();
        isEnabled = true;
        foreach(Stage s in stages)
        {
            AddStage(s);
        }
    }

    void AddStage(Stage stage)
    {
        stages.Add(stage);
    }




}
