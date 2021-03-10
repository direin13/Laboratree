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

    public void CrossBreed()
    {
        PlantManager plantManager = GameObject.Find("GameManager").GetComponent<PlantManager>();        //get plant manager object
        int index1 = plantDisplay1.GetComponent<PlantDisplay>().indexNum;           //index of plants in collection
        int index2 = plantDisplay2.GetComponent<PlantDisplay>().indexNum;
        string name = inputField.GetComponent<TMP_InputField>().text;               //user input field

        //handle errors - not enough plants in collection, crossbreeding same breed, invalid/existing name input
        string invMessage = "";
        if (plantManager.plantCollection.Count <= 1)
        {
            invMessage = "There aren't a enough plants in the collection";
        }
        else if (index1 == index2)
        {
            invMessage = "cannot crossbreed the same plant";
        }
        else if (name.Trim() == "")
        {
            invMessage = "Invalid name";
        }

        else
        {
            try
            {
                plantManager.Breed(plantManager.plantCollection[index1], plantManager.plantCollection[index2], name);
            }
            catch (ArgumentException)
            {
                invMessage = "A plant with this name already exists in the collection";
            }
        }

        //show pop up panel to confirm/deny crossbreed status
        PopUpManager popUpManager = GameObject.Find("GameManager").GetComponent<PopUpManager>();
        if (invMessage != "")
        {
            popUpManager.PopUpMessage(invMessage);
        }
        else
        {
            popUpManager.PopUpMessage(String.Format("'{0}' has been added to the collection", name));
        }
    }
}
