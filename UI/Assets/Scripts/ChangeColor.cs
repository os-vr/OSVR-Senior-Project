using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple script to change the color of an object
/// </summary>

public class ChangeColor : MonoBehaviour
{
    Renderer rend;
    public Color[] colors;
    // Start is called before the first frame update
    void Start()
    {
        rend = this.GetComponent<Renderer>();    
    }
    public void ColorChange(int color)
    {
        rend.material.color = colors[color];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
