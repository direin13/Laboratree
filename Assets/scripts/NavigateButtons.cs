using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigateButtons : MonoBehaviour
{

    public bool clicked;

    // Start is called before the first frame update
    void Start()
    {
        clicked = false;        //set false by default
    }

    public void OnMouseDown()
    {
        clicked = true;         //set to true when clicked
    }

}
