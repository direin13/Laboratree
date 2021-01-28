using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.Events;

public class MouseInteraction : MonoBehaviour
{
    public bool isHovering;
    public UnityEvent onMouseClick;

    void Awake()
    {
        if (onMouseClick == null)
            onMouseClick = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        isHovering = false;
    }

    public void OnMouseDown()
    {
        print(transform.root.gameObject.name + "Clicked");
        onMouseClick.Invoke();
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
