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

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire1"))
        {
            gameObject.layer = collLayer;
            
        }
        else
        {
           gameObject.layer = initLayer;
        }
        
    }
}
