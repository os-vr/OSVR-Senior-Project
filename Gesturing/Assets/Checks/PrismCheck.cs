using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismCheck : Check {

    private Vector3 position1;
    private Vector3 position2;
    private BoxCollider boxCollider;
    private GameObject box;
    public GStatus passValue;
    public GStatus failValue;

    public PrismCheck(Vector3 pos1, Vector3 pos2, GStatus passValue, GStatus failValue)
    {
        this.position1 = pos1;
        this.position2 = pos2;
        this.passValue = passValue;
        this.failValue = failValue;

        
    }

    public GStatus CheckPoint(GTransform gTransform)
    {

        Vector3 pos = gTransform.position;
        bool x = pos.x >= Mathf.Min(position1.x, position2.x) && pos.x <= Mathf.Max(position1.x, position2.x);
        bool y = pos.y >= Mathf.Min(position1.y, position2.y) && pos.y <= Mathf.Max(position1.y, position2.y);
        bool z = pos.z >= Mathf.Min(position1.z, position2.z) && pos.x <= Mathf.Max(position1.z, position2.z);
        if (x && y && z)
        {
            return passValue;
        }
        return failValue;
    }
    public GStatus CheckAll(List<GTransform> transforms)
    {
        return GStatus.HALT;
    }
}
