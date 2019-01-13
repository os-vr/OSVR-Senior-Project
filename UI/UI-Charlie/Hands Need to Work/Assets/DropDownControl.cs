using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DropDownControl : MonoBehaviour {
    public class Option{
        public string name;
        public Image image;
        public GameObject gameobject;
        Option(string name, Image image)
        {

        }
        Option(string name, GameObject gameObject)
        {

        }
    }
    public GameObject template;
    public float changeInHeight;
    public Sprite flatBox;
    public GameObject fullBox;
    public GameObject boundingBox;
    public bool threeD;
    public string[] options;
    public UnityEvent onDropClick;
    public UnityEvent onSelectClick;
    public UnityEvent onHover;
    public UnityEvent onEnter;
    public UnityEvent onExit;
    public class valueChangeEvent : UnityEvent<int> { }
    public valueChangeEvent onValueChange;
    bool onepress;

    // Use this for initialization
    void Start () {
        OuchMeRibs.pressed += press;
        if (threeD)
        {
            //Instantiate(fullBox, transform.position, transform.rotation);
        }
        else
        {
            GameObject go = new GameObject();
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = flatBox as Sprite;
            go.transform.position = this.transform.position;
            go.transform.rotation = this.transform.rotation;
        }
        onepress = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKey && !onepress && false)
        {
            onepress = true;
            activate_dropdown();
        }
	}
    void activate_dropdown()
    {
        print("on");
        float position = 1;
        print(boundingBox);
        Instantiate(boundingBox, this.transform.position, this.transform.rotation);
        foreach (string op in options)
        {
            GameObject inst = Instantiate(template, this.transform.position + /*this.transform.up*/new Vector3(0, 1, 0) * -position * changeInHeight, this.transform.rotation);
            TextMeshPro text = inst.GetComponentInChildren<TextMeshPro>();
            text.text = op;
            position++;
        }
    }
    void press(GameObject go)
    {
        print("pressed");
        activate_dropdown();
    }
    public void onValueChanged(int value)
    {
        print(value);
    }
}
