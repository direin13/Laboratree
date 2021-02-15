using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    private bool isHovering;
    public delegate void OnClick();
    // Start is called before the first frame update
    void Start()
    {
        isHovering = false;
    }

    public void OnMouseDown()
    {
        print(transform.root.gameObject.name + "Clicked");
    }

    public void OnMouseOver()
    {
        isHovering = true;
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (isHovering)
        {
            sr.color = new Color(sr.color[0], sr.color[1], sr.color[2], 0.15f);
        }
        else
        {
            sr.color = new Color(sr.color[0], sr.color[1], sr.color[2], 0f);
        }
        isHovering = false;
        if (transform.root.gameObject.name == "Aloe")
        {
        }
    }
}
