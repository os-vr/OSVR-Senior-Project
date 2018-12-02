using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage {

    private List<Check> checks;
	
    public Stage()
    {
        checks = new List<Check>();
        checks.Add(new RadiusCheck(new Vector3(0,2,0), 2));
        checks.Add(new LineCheck(new Vector3(0, 0, 0), new Vector3(0, 2, 0)));
    }


    public GSuccess Passes(GTransform transform)
    {
        GSuccess status;
        foreach(Check c in checks)
        {
            c.CheckPoint(transform);
        }
        return GSuccess.HALT;
    }
}
