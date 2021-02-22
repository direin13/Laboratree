using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public GameObject popUpCanvas;
    private int selectingPlant;
    // Start is called before the first frame update
    void Start()
    {
        popUpCanvas.SetActive(false);
        selectingPlant = -1;
    }

    public void PopUpMessage(string message)
    {
        popUpCanvas.transform.position = new Vector3(popUpCanvas.transform.position[0], popUpCanvas.transform.position[1], -60);
        popUpCanvas.GetComponent<PopUp>().PopUpMessage(message);
    }

    public void SwapPlant(int index)
    {
        //show the 3 active plants in collection to swap with
        PlantManager plantManager = GetComponent<PlantManager>();
        popUpCanvas.GetComponent<PopUp>().PopUpSelectPlant( plantManager.GetActivePlantIndexes() );
        selectingPlant = index;
    }

    // Update is called once per frame
    void Update()
    {
        PopUp popUp = popUpCanvas.GetComponent<PopUp>();
        if (popUp.confirmClicked)
        {
            popUp.confirmClicked = false;
            if (selectingPlant != -1)
            {
                //swapping selected plant into the labspace
                print("about to swap");
                PlantManager pManager = GetComponent<PlantManager>();

                pManager.SwapPlant(selectingPlant, popUp.selectBoxIndex);
                selectingPlant = -1;
            }
        }
        else if (!popUpCanvas.activeSelf)
        {
            selectingPlant = -1;
        }
    }
}
