using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMe : MonoBehaviour
{
    private float lastTime = -1f;
    private bool ToggleState = true;
    private TextMesh rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = this.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleActive()
    {
        if (Time.time - lastTime > 0.05)
        {
           
        ToggleState = !ToggleState;

            Color NewColor = rend.color;
            if (ToggleState)
                NewColor.a = 255f;
            else
            {
                NewColor.a = 0f;
            }
            rend.color = NewColor;
        }
        lastTime = Time.time;
    }
}
