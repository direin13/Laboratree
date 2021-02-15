using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddToCollection : MonoBehaviour
{
    [SerializeField]
    private GameObject plant, panel, GameManager;

    [SerializeField]
    private Button firstAddButton,secondAddButton,cancelButton;

    [SerializeField]
    private TextMeshProUGUI NameInput;

    public void showPanel(){
        panel.SetActive(true);
    }

    //second add button clicked
    public void addPlant(){
         if (NameInput.text != "") {
            GameObject newPlant = Instantiate(plant);
            newPlant.SetActive(false);
            newPlant.name = NameInput.text;
            GameManager.GetComponent<PlantManager>().plantCollection.Add(newPlant);
            panel.SetActive(false);
         }
    }

    public void cancelAdd(){
        panel.SetActive(false);
    }

}
