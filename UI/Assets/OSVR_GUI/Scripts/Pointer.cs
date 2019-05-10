using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public LayerMask initialLayer;
    public LayerMask layerForCollision;
    private int initLayer;
    private int collLayer;
    private LineRenderer pointerLine;
    private SphereCollider pointerCollider;
    public float maxDrawDistance = 10.0f;
    public OVRInput.Controller controllerType;
    public OVRInput.Button activationButton;

    void Start()
    {
        initLayer = (int)Mathf.Log(initialLayer.value, 2);
        collLayer = (int)Mathf.Log(layerForCollision.value, 2);

        gameObject.layer = initLayer;
        pointerLine = GetComponent<LineRenderer>();
        pointerCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        
        if(pointerLine != null) {
            RaycastHit info;
            Vector3 pos = gameObject.transform.position;
            Vector3 forward = gameObject.transform.forward;
            Vector3 hitPoint = pos + maxDrawDistance*forward;
            if (Physics.Raycast(new Ray(pos, forward), out info, maxDrawDistance, initLayer|collLayer)) {
                hitPoint = info.point;
            }

            pointerLine.SetPosition(0, pos);
            pointerLine.SetPosition(1, hitPoint);

            float height = Vector3.Distance(pos, hitPoint) / transform.lossyScale.z;
            pointerCollider.center = Vector3.forward * height;

        }

        if (OVRInput.Get(activationButton, controllerType))
        {
            gameObject.layer = collLayer;
        }
        else
        {
           gameObject.layer = initLayer;
        }
    }
}
