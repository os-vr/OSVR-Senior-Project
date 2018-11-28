using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage {

    List<Check> checks;
	
    GSuccess passes(GTransform transform)
    {
        GSuccess status;
        foreach(Check c in checks)
        {

        }
        return GSuccess.HALT;
    }
}
