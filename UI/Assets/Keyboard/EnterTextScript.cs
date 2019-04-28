using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OSVR.UI
{
    public class EnterTextScript : MonoBehaviour
    {
        public Text text;
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
            // if (timer <= 0.0)
            //  {
            //      timer = 0.25f;
            //  }

            //     timer -= Time.deltaTime;

            // Debug.Log(timer);

        }

        public void AddText()
        {
            if (Time.time - timer > timeDelay)
            {
                text.text += "\r\n";
                timer = Time.time;
            }



        }
    }
}