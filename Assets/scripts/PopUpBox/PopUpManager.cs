using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public GameObject popUpCanvas;
    // Start is called before the first frame update
    void Start()
    {
        popUpCanvas.SetActive(false);
    }

    public void PopUpMessage(string message)
    {
        popUpCanvas.transform.position = new Vector3(popUpCanvas.transform.position[0], popUpCanvas.transform.position[1], -90);
        popUpCanvas.GetComponent<PopUp>().PopUpMessage(message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
