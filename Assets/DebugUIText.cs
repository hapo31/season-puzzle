using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUIText : MonoBehaviour {

    private Canvas canvas = null;
    private Text text = null;

    private int i = 0;

    public string Text
    {
        set
        {
            if (canvas != null)
            {
                text.text = value;
            }
        }
    }

	// Use this for initialization
	void Start () {
        canvas = GetComponentInChildren<Canvas>();
        text = canvas.GetComponentInChildren<Text>();

    }
	
	// Update is called once per frame
	void Update () {
        Text = $"{i} frames {Input.GetAxis("Vertical")}";
        ++i;
	}
}
