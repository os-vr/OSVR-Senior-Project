using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismCheck : Check {

    private Vector3 position1;
    private Vector3 position2;
    private BoxCollider boxCollider;
    private GameObject box;
    public GSuccess passValue;
    public GSuccess failValue;

    public PrismCheck(Vector3 pos1, Vector3 pos2)
    {
        this.position1 = pos1;
        this.position2 = pos2;

        this.VisualizeCheck();

        
    }

    public GSuccess CheckPoint(GTransform gTransform)
    {
        if (boxCollider.bounds.Contains(gTransform.position))
        {
            box.GetComponent<Renderer>().material.SetColor("_WireColor", Color.red);
            Debug.Log("Entered the cube");
            return passValue;
        }
        box.GetComponent<Renderer>().material.SetColor("_WireColor", Color.blue);

        return failValue;
    }


    private void VisualizeCheck()
    {
        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boxCollider = box.GetComponent<BoxCollider>();

        box.GetComponent<Renderer>().material = new Material(Shader.Find("WireframeShader"));
        box.transform.position = (position1 + position2)/2;
        box.transform.localScale = position2 - position1;



    }
}
