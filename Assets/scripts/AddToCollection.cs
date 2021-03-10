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

    public void showPanel(){
        panel.SetActive(true);
    }

    //second add button clicked
    public void addPlant(){

        var text = NameInput.text;

        if (text.Length == 0) {
            GameManager.GetComponent<PopUpManager>().PopUpMessage("Invalid name");
        } else {
            try {
                GameManager.GetComponent<PlantManager>().MakePlant(NameInput.text, plantPrefab.name);
                NameInput.text = "";
            } catch (ArgumentException e) {
                Debug.Log("Exception caught: " + e);
            }

        }

        panel.SetActive(false);

    }

    public void cancelAdd(){
        NameInput.text="";
        panel.SetActive(false);
    }

}
