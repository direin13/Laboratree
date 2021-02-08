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
    public List<GameObject> plantList;
    private GameObject currPlant;
    
    public int indexNum;

    float getCurrVal(string attribute) {
        print("current index num is " + indexNum.ToString());
        return plantList[indexNum].transform.Find("Dependencies/" + attribute).GetComponent<DependenceAttribute>().currValue;
    }

    GameObject makeClone(){
        GameObject plant = Instantiate(plantList[indexNum]);
        plant.transform.localScale = new Vector3(35,35,1);
        plant.transform.position = new Vector3(70,-112,-20);
        plant.AddComponent<FakeFollow>();
        return plant;
    }

    void Start(){
        GameObject plantCollectionHolder = GameObject.Find("GameManager");
        PlantManager plantManager = plantCollectionHolder.GetComponent<PlantManager>();
        plantList = plantManager.plantCollection;
        indexNum = 0;   //starts at 0

        display();
    }

    void display(){

        currPlant = makeClone();

        //change name to current plant
        nameText.text = "Name: " + plantList[indexNum].name;

        //change attribute values
        var lighting = getCurrVal("Lighting").ToString();
        var temp = getCurrVal("Temperature").ToString();
        var water = getCurrVal("Water").ToString();
        var fertiliser = getCurrVal("Fertiliser").ToString();

        //set text in interval fields
        lightInput.text = lighting;
        tempInput.text = temp;
        waterInput.text = water;
        fertiliserInput.text = fertiliser;

    }

    void Update(){

        //switch to next/previous plant
        if (leftButton.GetComponent<NavigateButtons>().clicked == true) {
            navigate(false,leftButton);
        } 

        if (rightButton.GetComponent<NavigateButtons>().clicked == true) {
            navigate(true,rightButton);
        }
        
        //set index amount
        indexText.text = (indexNum + 1).ToString() + "/" + plantList.Count.ToString();
        // print("current lighting val is " + getCurrVal("Lighting").ToString());
    }

    //displays and navigates between plants in list
    void navigate(bool next, Button button) {

        try {
            //change index
            if (next) {
                indexNum++;     //add one to index
            } else {
                indexNum--;     //subtract one from index
            }

            var trigger = plantList[indexNum];      //triggers catch
            
        } catch (System.ArgumentOutOfRangeException e1){      //when index is out of range
            if (indexNum >= plantList.Count) {       //restarts
                indexNum = 0;

            } else if (indexNum <= 0) {
                indexNum = plantList.Count - 1;
            }

            Debug.Log("Exception caught: " + e1);

        }     
        finally {
            Destroy(currPlant);
            button.GetComponent<NavigateButtons>().clicked = false;         //reset button status
            display();
        }

    }

}
