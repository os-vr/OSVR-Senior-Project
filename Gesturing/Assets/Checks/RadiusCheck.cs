using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusCheck : Check
{

    private Vector3 position;
    private float radius;
    private GameObject sphere;
    private SphereCollider sphereCollider;

    public RadiusCheck(Vector3 position, float radius)
    {
        this.position = position;
        this.radius = radius;

        this.VisualizeCheck();

        sphere.GetComponent<Renderer>().enabled = false;
    }

    public GStatus CheckPoint(GTransform gTransform)
    {
        //sphere.GetComponent<Renderer>().enabled = true;
        if(sphereCollider.bounds.Contains(gTransform.position))
        {
            //sphere.GetComponent<Renderer>().material.SetColor("_WireColor", Color.red);
            //sphere.GetComponent<Renderer>().enabled = false;
            return GStatus.PASS;
        }

        sphere.GetComponent<Renderer>().material.SetColor("_WireColor", Color.blue);

        return GStatus.CONTINUE;
    }


    private void VisualizeCheck()
    {
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereCollider = sphere.GetComponent<SphereCollider>();
        sphere.GetComponent<Renderer>().enabled = false;
        sphere.GetComponent<Renderer>().material = new Material(Shader.Find("WireframeShader"));
        sphere.transform.position = position;

        sphere.transform.localScale = new Vector3(radius, radius, radius);


        
    }
    public GStatus CheckAll(List<GTransform> transforms)
    {
        return GStatus.HALT;
    }

}
