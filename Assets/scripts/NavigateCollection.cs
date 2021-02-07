using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class NavigateCollection : MonoBehaviour
{
    public TextMeshProUGUI indexText;

    [SerializeField]
    private Button leftButton, rightButton;
    
    [SerializeField]
    private TextMeshProUGUI nameText,lightInput,tempInput,waterInput,fertiliserInput;

    public Transform parent;
    private List<GameObject> originalPlantList;
    
    public List<GameObject> plantList = new List<GameObject>();     //for creating a deep copy of original list 
    public int indexNum = 0;

    float getCurrVal(string attribute) {
        return plantList[indexNum].transform.Find("Dependencies/" + attribute).GetComponent<DependenceAttribute>().currValue;
    }

    void Start(){
        GameObject plantCollectionHolder = GameObject.Find("GameManager");
        PlantManager plantManager = plantCollectionHolder.GetComponent<PlantManager>();
        originalPlantList = plantManager.plantCollection;
        indexNum = 0;   //starts at 0

        // display first plant in the list
        for (int i = 0; i < originalPlantList.Count; i++) {
            //instantiate object - set position, rotation
            //use new Vector3(70,-100,-20) when working with image

            GameObject plant = Instantiate(originalPlantList[i]);   //create clone
            plant.transform.localScale = new Vector3(35,35,1);
            plant.transform.position = new Vector3(70,-112,-20);
            plant.SetActive(false);     //make them inactive so they do not show on collection
            plant.name = plant.name.Replace("(Clone)", "");     //remove clone indicator

            
            //when instantiated, the object is not a child of any object or any canvas
            //this is used to make the sprite appear on the screen
            //FakeFollow will set the Plant Collection Page its fake parent object        
            //script will make it move to follow folder as if it were its child
            plant.AddComponent<FakeFollow>();

            plantList.Add(plant);       //add clone to local copy
        }

        display();
    }

    void display(){

        //change scale
        // plantList[indexNum].transform.localScale = new Vector3(35,35,1);
        // plantList[indexNum].transform.position = new Vector3(70,63,-20);
        plantList[indexNum].SetActive(true);    //make plant visible

        //change name to current plant
        nameText.text = "Name: " + plantList[indexNum].name;

        //change attribute values
        var lighting = getCurrVal("Lighting").ToString();
        var temp = getCurrVal("Temperature").ToString();
        var water = getCurrVal("Water").ToString();
        var fertiliser = getCurrVal("Fertiliser").ToString();

        print("Current lighting value is: " + lighting);

        //set text in interval fields
        lightInput.text = lighting;
        tempInput.text = temp;
        waterInput.text = water;
        fertiliserInput.text = fertiliser;

        //set index amount
        indexText.text = (indexNum + 1).ToString() + "/" + plantList.Count.ToString();
        // print(plantList[indexNum].transform.position);

        //make it stop responding to time
        plantList[indexNum].GetComponent<Timer>().getTicks = false;
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

            // //set plant at new index active
            // plantList[indexNum].SetActive(true);

            // //change name
            // // nameText.text = "Name: " + plantList[indexNum].name;

            // //change attribute values


            // indexText.text = (indexNum + 1).ToString() + "/" + plantList.Count.ToString();
        } catch (System.ArgumentOutOfRangeException e1){      //when index is out of range
            if (indexNum == plantList.Count) {       //restarts
                indexNum = 0;

            } else if (indexNum <= 0) {
                indexNum = plantList.Count - 1;
            }

            Debug.Log("Exception caught: " + e1);
        }     
        finally {
            button.GetComponent<NavigateButtons>().clicked = false;         //reset button status
            display();
        }

    }

}
