/// <summary>
/// Script attachable to a keyboard key that enables that key create a new line
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OSVR.UI
{
    public class EnterTextScript : MonoBehaviour
    {
        /// <summary>
        /// Text object that the script will modify
        /// </summary>
        public Text text;
        /// <summary>
        /// Timer for input delay
        /// </summary>
        private float timer = -1;
        /// <summary>
        /// User specified float for seconds to delay before accepting more input
        /// </summary>
        public float timeDelay = 0.25f;
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
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