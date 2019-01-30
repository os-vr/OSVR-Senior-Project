using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScript : MonoBehaviour
{
    public Text text;
    public char key;
    private bool needDelay;
    private float timer = -1;
    public float timeDelay = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        needDelay = false;
    }

    // Update is called once per frame
    void Update()
    {
        //int len = GameObject.FindGameObjectsWithTag("IsClicked").Length;
        //Debug.Log(len);
        //if (len > 0)
        //{
        //    needDelay = true;
        //}

        //if (len == 0)
        //{
        //    needDelay = false;
        //}

    }

    public void AddText()
    {

      if (Time.time - timer > timeDelay)
        {
            text.text += key;
            timer = Time.time;
        }
      
      
            
        
    }
}
