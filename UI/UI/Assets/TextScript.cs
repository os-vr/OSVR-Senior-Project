using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScript : MonoBehaviour
{
    public Text text;
    public char key;
    private bool needDelay;
    private float timer = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        needDelay = false;
    }

    // Update is called once per frame
    void Update()
    {
       // if (timer <= 0.0)
       //  {
       //      timer = 0.25f;
       //  }
        
       //     timer -= Time.deltaTime;

       // Debug.Log(timer);

    }

    public void AddText()
    {
      
                text.text += key;
            
        
    }
}
