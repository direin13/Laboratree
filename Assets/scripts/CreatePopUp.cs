//can be removed from repo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePopUp : MonoBehaviour
{
    private GameObject panel;

    public void popUp()
    {
        panel = new GameObject("Panel");
        panel.AddComponent<CanvasRenderer>();
        panel.transform.position = new Vector3(350,153,0);
        // Image i = panel.AddComponent<Image>();
        // i.color = Color.blue;
    }

}
