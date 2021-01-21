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
        panel.transform.position = new Vector2(350,153);
        // Image i = panel.AddComponent<Image>();
        // i.color = Color.blue;
    }

}
