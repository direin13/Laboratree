using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class CrossBreedClick : MonoBehaviour
{
    public GameObject plantDisplay1, plantDisplay2;
    public GameObject inputField;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void CrossBreed()
    {
        PlantManager plantManager = GameObject.Find("GameManager").GetComponent<PlantManager>();
        int index1 = plantDisplay1.GetComponent<PlantDisplay>().indexNum;
        int index2 = plantDisplay2.GetComponent<PlantDisplay>().indexNum;
        string name = inputField.GetComponent<TMP_InputField>().text;

        string invMessage = "";
        if (plantManager.plantCollection.Count <= 1)
        {
            invMessage = "There aren't a enough plants in the collection";
        }
        else if (index1 == index2)
        {
            invMessage = "cannot crossbreed the same plant";
        }

        else
        {
            try
            {
                plantManager.Breed(plantManager.plantCollection[index1], plantManager.plantCollection[index2], name);
            }
            catch (ArgumentException)
            {
            }
        }

        PopUpManager popUpManager = GameObject.Find("GameManager").GetComponent<PopUpManager>();
        if (invMessage != "")
        {
            popUpManager.PopUpMessage(invMessage);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
