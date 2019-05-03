using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OSVR.UI
{
/// <summary>
/// Script attachable to a keyboard key that enables that key add onto a text object any user specified key
/// </summary>
    public class TextScript : MonoBehaviour
    {
        /// <summary>
        /// Text object that the script will modify
        /// </summary>
        public Text text;
        /// <summary>
        /// User specifies character that the key will input
        /// </summary>
        public char key;
        /// <summary>
        /// Timer for input delay
        /// </summary>
        private float timer = -1;
        /// <summary>
        /// User specified float for seconds to delay before accepting more input
        /// </summary>
        public float timeDelay = 0.25f;
        public enum Text_Type {Normal, Enter, Backspace};
        public Text_Type TextType;
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

                switch (TextType)
                {
                    case Text_Type.Backspace:
                        text.text = text.text.Substring(0, text.text.Length - 1);
                        break;
                    case Text_Type.Enter:
                        text.text += "\r\n";
                        break;
                    default:
                        text.text += key;
                        break;
                }
                timer = Time.time;
            }
        }
    }
}
