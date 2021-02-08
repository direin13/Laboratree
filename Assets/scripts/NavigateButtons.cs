using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigateButtons : MonoBehaviour
{

    public bool clicked;

    // Start is called before the first frame update
    void Start()
    {
        clicked = false;
    }

    public void OnMouseDown()
    {
        // print("I WAS CLICKED!!");
        clicked = true;
    }

}
