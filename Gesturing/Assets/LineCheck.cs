using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCheck : Check
{
    private Vector3 firstPosition;
    private Vector3 secondPosition;
    private GameObject line;
    private BoxCollider coll;

    public LineCheck(Vector3 firstPosition, Vector3 secondPosition)
    {
        this.firstPosition  = firstPosition;
        this.secondPosition = secondPosition;

        this.VisualizeCheck();
    }

    public GSuccess CheckPoint(GTransform gTransform)
    {
        return GSuccess.CONTINUE;
    }


    private void VisualizeCheck()
    {
        //line = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //this.lineRenderer = line.AddComponent<LineRenderer>();
        //lineRenderer.material = new Material(Shader.Find("WireframeShader"));
        //lineRenderer.SetPositions(new Vector3[] { firstPosition, secondPosition });
        //lineRenderer.widthMultiplier = .2f;
    }


}
