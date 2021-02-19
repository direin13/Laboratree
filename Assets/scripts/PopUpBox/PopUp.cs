using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PopUp : MonoBehaviour
{
    public GameObject messageBox;
    public GameObject plantSelectBox;
    public int selectBoxIndex;
    public GameObject darkPanel;
    public bool confirmClicked;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void SetBoxActive(GameObject box)
    {
        foreach (Transform t in transform)
        {
            if (t != transform && t.gameObject != darkPanel)
            {
                if (t.gameObject == box)
                {
                    box.SetActive(true);
                }
                else
                {
                    t.gameObject.SetActive(false);
                }
            }
        }
    }

    public void PopUpMessage(string message)
    {
        TMP_Text tmpro = messageBox.GetComponentInChildren<TMP_Text>();
        tmpro.text = message;
        gameObject.SetActive(true);
        SetBoxActive(messageBox);
        confirmClicked = false;
    }

    public void PopUpSelectPlant(int[] plantIndexes)
    {
        //displays 3 plants
        print(plantIndexes[0].ToString() + plantIndexes[1].ToString() + plantIndexes[2].ToString());
        selectBoxIndex = -1;
        gameObject.SetActive(true);
        PlantDisplay[] displays = plantSelectBox.GetComponentsInChildren<PlantDisplay>();
        for (int i=0; i < plantIndexes.Length; i++)
        {
            if (displays[i] != null)
            {
                displays[i].ChangeIndex(plantIndexes[i]);
            }
        }

        SetBoxActive(plantSelectBox);
        confirmClicked = false;
    }

    public void OnConfirmButtonDown()
    {
        confirmClicked = true;
        gameObject.SetActive(false);
    }

    public void OnCancelButtonDown()
    {
        confirmClicked = false;
        gameObject.SetActive(false);
    }

    public void OnPlantSelectDown(GameObject plantDisplay)
    {
        selectBoxIndex = plantDisplay.GetComponent<PlantDisplay>().indexNum;
    }

    public void OnDisable()
    {
        TMP_Text tmpro = messageBox.GetComponentInChildren<TMP_Text>();
        tmpro.text = "";
    }


    // Update is called once per frame
    void Update()
    {
    }
}
