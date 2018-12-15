using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage {

    private List<Check> checks;
    private GSuccess successValue;
    private GSuccess failValue;
	
    public Stage()
    {
        checks = new List<Check>();
        successValue = GSuccess.PASS;
        failValue = GSuccess.CONTINUE;
    }

    public Stage(List<Check> cs)
    {
        checks = cs;
        successValue = GSuccess.PASS;
        failValue = GSuccess.CONTINUE;
    }

    public Stage(List<Check> cs, GSuccess pass, GSuccess fail)
    {
        checks = cs;
        successValue = pass;
        failValue = fail;
    }


    public GSuccess Passes(GTransform transform)
    {
        bool allPassing = true;
        foreach(Check c in checks)
        {
            GSuccess checkStatus = c.CheckPoint(transform);
            if (checkStatus == GSuccess.HALT)
            {
                return checkStatus;
            }
            else if (checkStatus == GSuccess.CONTINUE)
            {
                allPassing = false;
            }
        }
        if (allPassing)
        {
            return successValue;
        }
        return failValue;
    }
}
