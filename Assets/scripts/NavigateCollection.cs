using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class NavigateCollection : MonoBehaviour
{
    // public GameObject plantManager;
    public TextMeshProUGUI indexText;
    public Button leftButton;
    public Button rightButton;
    public Transform parent;
    public TextMeshProUGUI nameText;
    private List<GameObject> plantList;
    private int indexNum;

    void Start(){
        GameObject plantCollectionHolder = GameObject.Find("GameManager");
        PlantManager plantManager = plantCollectionHolder.GetComponent<PlantManager>();
        plantList = plantManager.plantCollection;
        indexNum = 0;   //starts at 0

        //when instantiated, the object is not a child of any object or any canvas so no images will appear on screen
        //Setting the parent to Plant Display object        
        // display first plant in the list
        for (int i = 0; i <plantList.Count; i++) {
            //instantiate object - set position, rotation, and parent
            GameObject plant = Instantiate(plantList[i], new Vector3(70,-100,-20),Quaternion.identity, parent);

            plant.SetActive(false);     //make them inactive so they do not show on collection
        }
        display();
    }

    void display(){

        plantList[indexNum].SetActive(true);    //make plant visible

        //change name to current plant
        nameText.text = "Name: " + plantList[indexNum].name;

        //set index amount
        indexText.text = (indexNum + 1).ToString() + "/" + plantList.Count.ToString();

    }

    void Update(){

        //switch to next/previous plant
        if (leftButton.GetComponent<NavigateButtons>().clicked == true) {
            navigate(false,leftButton);
        } 

        if (rightButton.GetComponent<NavigateButtons>().clicked == true) {
            navigate(true,rightButton);
        }
    }

    //displays and navigates between plants in list
    void navigate(bool next, Button button) {

        try {
            //make current plant inactive
            plantList[indexNum].SetActive(false);
            
            //change index
            if (next) {
                indexNum++;     //add one to index
            } else {
                indexNum--;     //subtract one from index
            }

            //set plant at new index active
            plantList[indexNum].SetActive(true);

            //change name
            nameText.text = "Name: " + plantList[indexNum].name;

            indexText.text = (indexNum + 1).ToString() + "/" + plantList.Count.ToString();
            button.GetComponent<NavigateButtons>().clicked = false;         //reset button status        

        } catch (System.ArgumentOutOfRangeException e1){      //when index is out of range
            if (indexNum == plantList.Count) {       //restarts
                indexNum = 0;

            } else if (indexNum <= 0) {
                indexNum = plantList.Count - 1;
            }

            Debug.Log("Exception caught: " + e1);
            button.GetComponent<NavigateButtons>().clicked = false;         //reset button status
            display();
            
        }     
        // finally {}      //do nothing

    }

}
