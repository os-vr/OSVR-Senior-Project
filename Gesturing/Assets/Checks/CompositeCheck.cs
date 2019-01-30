using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeCheck : Check{

    private List<Check> checks;
    private GStatus successValue;
    private GStatus failValue;
	
    public CompositeCheck()
    {
        checks = new List<Check>();
        successValue = GStatus.PASS;
        failValue = GStatus.CONTINUE;
    }

    public CompositeCheck(List<Check> cs)
    {
        checks = cs;
        successValue = GStatus.PASS;
        failValue = GStatus.CONTINUE;
    }

    public CompositeCheck(List<Check> cs, GStatus pass, GStatus fail)
    {
        checks = cs;
        successValue = pass;
        failValue = fail;
    }


    public GStatus CheckPoint(GTransform transform)
    {
        bool allPassing = true;
        foreach(Check c in checks)
        {
            GStatus checkStatus = c.CheckPoint(transform);
            if (checkStatus == GStatus.HALT)
            {
                return checkStatus;
            }
            else if (checkStatus == GStatus.CONTINUE)
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

    public GStatus CheckAll(List<GTransform> transforms)
    {
        return GStatus.HALT;
    }
}
