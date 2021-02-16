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
        } else {
        GameManager.GetComponent<PlantManager>().MakePlant(NameInput.text, plantPrefab.name);

        }
    }

    public void cancelAdd(){
        panel.SetActive(false);
    }

}
