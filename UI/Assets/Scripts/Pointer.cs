using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public LayerMask initialLayer;
    public LayerMask layerForCollision;
    private CharacterController cc;
    private int initLayer;
    private int collLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        initLayer = (int)Mathf.Log(initialLayer.value, 2);
        collLayer = (int)Mathf.Log(layerForCollision.value, 2);

        gameObject.layer = initLayer;
        cc = GetComponent<CharacterController>();
        GameObject hand = GameObject.Find("hand_right");
        gameObject.transform.parent = hand.transform;

    }

    // Update is called once per frame
    void Update()
    {
        GameObject go = GameObject.Find("hands:b_r_index_ignore");
        if(go != null) {
            Vector3 newPos = go.transform.position + transform.up*transform.localScale.y;
            transform.position = newPos;
        }
        
        if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            gameObject.layer = collLayer;
        }
        else
        {
           gameObject.layer = initLayer;
        }
    }
}
