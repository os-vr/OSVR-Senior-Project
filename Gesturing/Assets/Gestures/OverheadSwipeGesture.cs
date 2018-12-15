using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadSwipeGesture : Gesture{

    public OverheadSwipeGesture(Vector3 start, Vector3 end, float radius, Normalizer n) : base(n)
    {
        List<Check> startChecks = new List<Check>();
        startChecks.Add(new RadiusCheck(start, radius));
        //TODO: generalize this for the two passed in points

        List<Check> endChecks = new List<Check>();
        PrismCheck p = new PrismCheck(new Vector3(0, 0, 0), new Vector3(1, .85f, .5f));
        p.passValue = GSuccess.HALT;
        p.failValue = GSuccess.PASS;
        endChecks.Add(p);
        endChecks.Add(new RadiusCheck(end, radius));

        Stage s = new Stage(startChecks);
        Stage e = new Stage(endChecks);
        AddStage(s);
        //TODO: add exclusion zone
        AddStage(e);

    }
}
