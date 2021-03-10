using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddToCollection : MonoBehaviour
{
    [SerializeField]
    private GameObject plantPrefab, panel, GameManager;

    [SerializeField]
    private Button firstAddButton,secondAddButton,cancelButton;

    [SerializeField]
    private TMP_InputField NameInput;

    //make hidden panel visible to user
    public void showPanel(){
        panel.SetActive(true);
    }

    //second add button clicked, confirming add
    public void addPlant(){

        var text = NameInput.text;

        if (text.Length == 0) {
            GameManager.GetComponent<PopUpManager>().PopUpMessage("Invalid name");      //make sure name is not empty
        } else {
            //try add plant with new name to collection, catch if plant with same name already exists
            try {
                GameManager.GetComponent<PlantManager>().MakePlant(NameInput.text, plantPrefab.name);
                GameManager.GetComponent<PopUpManager>().PopUpMessage(String.Format("'{0}' has been added to the collection", NameInput.text));
            } catch (ArgumentException e) {
                GameManager.GetComponent<PopUpManager>().PopUpMessage(String.Format("'{0}' is already in the collection", NameInput.text));
                Debug.Log("Exception caught: " + e);
            }

            NameInput.text = "";    //clear input field
        }

        panel.SetActive(false);     //hide panel from users

    }

    //cancel button clicked, clearing and hiding panels
    public void cancelAdd(){
        NameInput.text="";
        panel.SetActive(false);
    }

}
