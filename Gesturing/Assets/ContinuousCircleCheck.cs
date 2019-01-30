using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinusCircleCheck : Check {
    List<GTransform> points;
    float inferredRadius;
    float toleranceRate;
    int minPoints;

    public ContinusCircleCheck()
    {
        points = new List<GTransform>();
    }

    // Use this for initialization
    public GStatus CheckPoint(GTransform gTransform)
    {
        // add points to the list
        points.Add(gTransform);

        //while there is a less than minimum number of points, keep adding points
        if (points.Count < minPoints)
        {
            return GStatus.CONTINUE;
        }
        //after minimum, check to see if any point touches an existing one
        float tolerane = 0.01f;
        for (int i = 0; i < points.Count - 1; i++)
        {
            
            Vector3 point = points[i].position;
            if (Vector3.Distance(point, gTransform.position) < tolerane)
            {
                bool result = CheckCircle(i);
                if (result)
                {
                    return GStatus.PASS;
                }
            }
        }
        return GStatus.CONTINUE;
    }

    public GStatus CheckAll(List<GTransform> buffer)
    {
        return GStatus.HALT;
    }

    private bool CheckCircle(int startingIndex)
    {
        return false;
    }
}
