using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CompositeGesture : Gesture {
    private List<Gesture> gestures;
	// Use this for initialization
	public CompositeGesture(List<Gesture> gestures)
    {
        this.gestures.AddRange(gestures);
    }
    
    public override bool CheckStages(GTransform gtr)
    {

        if (currentStage >= gestures.Count)
        {
            Debug.Log("Gesture complete");
            CompleteGesture(null);
            currentStage = 0;
            return true;
            //isEnabled = false;
        }
        else
        {
            if (gestures[currentStage].CheckStages(gtr))
            {
                currentStage++;
            }
        }
        return false;
    }
}
